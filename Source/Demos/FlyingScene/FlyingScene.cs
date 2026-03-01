using System.Numerics;
using SoftwareRasterizer.Helpers;
using SoftwareRasterizer.Shaders;
using SoftwareRasterizer.Types;
using static System.MathF;
using static SoftwareRasterizer.Maths;

namespace SoftwareRasterizer.Demo;

public class FlyingScene : Scene
{
	public static Vector3 dirToSun = Vector3.Normalize(new(0.3f, 1f, 0.6f));
	const float terrainRes = 35;
	const float terrainChunkSize = 50;
	readonly TerrainShader terrainShader = new(dirToSun);
	readonly List<Model> terrainChunksActive = new();
	readonly Dictionary<(int x, int y), Model> terrainChunkLookup = new();
	const float cloudLifeMax = 35;
	readonly Model boy;
	readonly Model fox;
	readonly Model propeller;
	readonly AirplaneController airplaneController;
	readonly Vector3 skyCol = new Vector3(131, 188, 243) / 255;
	readonly CloudData[] clouds;
	readonly Random rng = new (1024);


	class CloudData
	{
		public Model model;
		public float lifeTime;
		public float scale;
	}

	public FlyingScene()
	{
		// Load shaders
		LitTextureShader boyShader = new(dirToSun, ResourceHelper.LoadTexture("daveTex"));
		LitTextureShader paletteShader = new(dirToSun, ResourceHelper.LoadTexture("colMap"));
		CloudShader cloudShader = new(dirToSun, Vector3.One);

		terrainShader.SkyCol = skyCol;
		cloudShader.AtmosCol = skyCol;
		
		// Load models
		Model biplane = ResourceHelper.LoadModel("biplane", paletteShader);
		airplaneController = new AirplaneController(biplane);
		
		boy = ResourceHelper.LoadModel("dave", boyShader);
		boy.Transform.Parent = airplaneController.model.Transform;
		boy.Transform.Scale = Vector3.One * 0.27f;
		boy.Transform.Position = new Vector3(0, -0.12f, -0.17f);

		fox = ResourceHelper.LoadModel("foxSitting", paletteShader);
		fox.Transform.Parent = airplaneController.model.Transform;
		fox.Transform.Scale = Vector3.One * 0.27f * 0.45f;
		fox.Transform.Position = new Vector3(0, -0.1f, -0.6f);

		propeller = ResourceHelper.LoadModel("propeller", paletteShader);
		propeller.Transform.Parent = airplaneController.model.Transform;

		// Create clouds
		clouds = new CloudData[60];

		for (int i = 0; i < clouds.Length; i++)
		{
			clouds[i] = new CloudData();
			clouds[i].model = ResourceHelper.LoadModel("cloud", cloudShader);
			NextCloudSpawn(clouds[i], true);
		}
	}

	float RandomUNorm() => (float)rng.NextDouble();

	Vector3 RandomInBox(Vector3 centre, Vector3 size)
	{
		float ox = size.X * (RandomUNorm() - 0.5f);
		float oy = size.Y * (RandomUNorm() - 0.5f);
		float oz = size.Z * (RandomUNorm() - 0.5f);

		return centre + new Vector3(ox, oy, oz);
	}

	Vector3 CameraPosition => Data.Camera.Transform.Position;


	void NextCloudSpawn(CloudData cloud, bool isFirstSpawn = false)
	{
		Vector3 spawnBoxSize = new(terrainChunkSize * 10, 30, terrainChunkSize * 10);
		Vector3 spawnBoxCentre = new(CameraPosition.X, 25, CameraPosition.Z);
		cloud.scale = Lerp(1, 3, RandomUNorm());
		cloud.model.Transform.Yaw = RandomUNorm() * PI * 2;
		cloud.model.Transform.Position = RandomInBox(spawnBoxCentre, spawnBoxSize);
		cloud.lifeTime = isFirstSpawn ? cloudLifeMax * RandomUNorm() : 0;
	}

	void UpdateCloud(CloudData cloud, float deltaTime)
	{
		// Scale cloud based on lifetime, and respawn if it has disappeared
		cloud.lifeTime += deltaTime;
		float scaleT = Min(cloud.lifeTime, cloudLifeMax - cloud.lifeTime) / 10;
		cloud.model.Transform.Scale = Vector3.One * cloud.scale * EaseCubeInOut(scaleT);
		if (cloud.lifeTime > cloudLifeMax) NextCloudSpawn(cloud);

		// Move with wind
		Vector3 windVelocity = new Vector3(0.35f, 0, -0.1f) * 3;
		cloud.model.Transform.Position += windVelocity * deltaTime;
	}


	// Test Scene
	public override void Update(RenderTarget renderTarget, float deltaTime)
	{
		renderTarget.Clear(skyCol);

		airplaneController.Update(deltaTime);
		propeller.Transform.Roll += -deltaTime * 12;
		UpdateCam(airplaneController);
		
		UpdateTerrainChunks(Data.Camera.Transform.Position, (int)terrainRes, terrainChunkSize);
		foreach (CloudData cloud in clouds)
		{
			UpdateCloud(cloud, deltaTime);
		}

		// Add active models to draw list for this frame
		Data.Models.Clear();
		
		Data.Models.AddRange(terrainChunksActive);
		Data.Models.Add(airplaneController.model);
		Data.Models.Add(boy);
		Data.Models.Add(fox);
		Data.Models.Add(propeller);

		foreach (CloudData cloud in clouds)
		{
			Data.Models.Add(cloud.model);
		}
	}

	public static float EaseQuadInOut(float t) => 3 * Square(Clamp01(t)) - 2 * Cube(Clamp01(t));
	public static float Square(float x) => x * x;
	public static float Cube(float x) => x * x * x;

	public static float EaseCubeInOut(float t)
	{
		t = Clamp01(t);
		int r = (int)System.Math.Round(t);
		return 4 * Cube(t) * (1 - r) + (1 - 4 * Cube(1 - t)) * r;
	}
	
	// Refresh list of terrain chunks to be drawn this frame
	void UpdateTerrainChunks(Vector3 camPos, int resolution, float chunkSize)
	{
		int centreX = (int)Round(camPos.X / chunkSize);
		int centreY = (int)Round(camPos.Z / chunkSize);
		terrainChunksActive.Clear();

		// Create grid of terrain chunks centered around camera position
		const int n = 5;
		int genCountThisFrame = 0;
		for (int y = centreY - n; y <= centreY + n; y++)
		{
			for (int x = centreX - n; x <= centreX + n; x++)
			{
				Vector3 c = new Vector3(x, 0, y) * chunkSize;
				if (Vector3.Dot(c - camPos, Data.Camera.Transform.Forward) < 0.3 && (Abs(y - centreY) > 1 || Abs(x - centreX) > 1)) continue;

				if (!terrainChunkLookup.TryGetValue((x, y), out Model chunk))
				{
					if (genCountThisFrame < 3)
					{
						Vector2 centre = new Vector2(x, y) * chunkSize; // chunk centre in (2D) world space
						chunk = new Model(TerrainGen.GenerateTerrain(resolution, chunkSize, centre), terrainShader);
					}

					genCountThisFrame++;
				}

				if (chunk != null)
				{
					terrainChunksActive.Add(chunk); // add chunk to draw list
					terrainChunkLookup[(x, y)] = chunk; // remember chunk to avoid re-generating
				}
			}
		}
	}


	public void UpdateCam(AirplaneController target)
	{
		Vector3 targetFwd = target.model.Transform.Forward;
		targetFwd.Y = 0;
		targetFwd = Vector3.Normalize(targetFwd);

		Data.Camera.Transform.Position = target.model.Transform.Position - targetFwd * 5 + new Vector3(0, 1.5f, 0);
		Data.Camera.Transform.SetRotation(0, target.model.Transform.Yaw, 0);
	}
}
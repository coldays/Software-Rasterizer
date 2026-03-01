using SoftwareRasterizer.Helpers;
using SoftwareRasterizer.Shaders;
using SoftwareRasterizer.Types;
using System.Numerics;

namespace SoftwareRasterizer.Demo;

public class TestScene : Scene
{
	FirstPersonCamera camController;

	public TestScene()
	{
		// Create shaders
		Vector3 dirToSun = Vector3.Normalize(new Vector3(0.3f, 1f, 0.6f));
		LitTextureShader floorShader = new(dirToSun, ResourceHelper.LoadTexture("uvGrid"));
		LitTextureShader boyShader = new(dirToSun, ResourceHelper.LoadTexture("daveTex"));
		LitShader dragonShader = new(dirToSun, new Vector3(82, 255, 190) / 255f);
		LitTextureShader paletteShader = new(dirToSun, ResourceHelper.LoadTexture("colMap"));

		// Load models
		Model boy = ResourceHelper.LoadModel("dave", boyShader);
		Model fox = ResourceHelper.LoadModel("fox", paletteShader);
		Model dragon = ResourceHelper.LoadModel("dragon", dragonShader);
		Model tree = ResourceHelper.LoadModel("tree", paletteShader);
		Model tree2 = ResourceHelper.LoadModel("tree", paletteShader);
		Model floor = ResourceHelper.LoadModel("floor", floorShader);
		Data.Models = [floor, boy, dragon, fox, tree, tree2];

		// Set positions, rotations, and scales
		boy.Transform.Yaw = -MathF.PI / 5;
		boy.Transform.Position = new Vector3(0, 0, 1) * -2.5f;
		fox.Transform.Position = new Vector3(0.7f, 0, -2.8f);
		fox.Transform.Scale = Vector3.One * 0.2075f;
		dragon.Transform.Yaw = MathF.PI / 3;
		dragon.Transform.Position = new Vector3(0, 0, 1) * 0.8f;
		tree.Transform.Position = new Vector3(-3.5f, 0, -1f);
		tree2.Transform.Position = new Vector3(4, 0, 3);

		Data.Camera.Transform.Position = new Vector3(0, 0, -4.5f);
		camController = new FirstPersonCamera(Data.Camera.Transform);
	}

	// Test Scene
	public override void Update(RenderTarget renderTarget, float deltaTime)
	{
		// Clear render target to black (and reset depth)
		Vector3 skyCol = new Vector3(167, 214, 250) / 255f;
		skyCol = Vector3.Zero;
		renderTarget.Clear(skyCol);

		camController.Update(deltaTime, renderTarget.Size);
	}
}
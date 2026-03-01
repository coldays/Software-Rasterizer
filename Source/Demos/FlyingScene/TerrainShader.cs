using SoftwareRasterizer.Types;
using System.Numerics;
using static System.MathF;

namespace SoftwareRasterizer.Demo;

public class TerrainShader(Vector3 directionToLight) : Shader
{
	public Vector3 DirectionToLight = directionToLight;
	public float[] Heights = [0, 0.6f, 2.5f, 12f]; // end height for each colour band (not including last)
	public Vector3 SkyCol;

	public Vector3[] Colours =
	[
		new(0.2f, 0.6f, 0.98f), // water
		new Vector3(235f, 205f, 94f) / 255, // sand
		new(0.2f, 0.6f, 0.1f), // grass
		new(0.5f, 0.35f, 0.3f), // mountain
		new(0.93f, 0.93f, 0.91f), // snow
	];

	public override Vector3 PixelColour(Vector2 pixelCoord, Vector2 texCoord, Vector3 normal, float depth)
	{
		// Get terrain colour from triangle's height
		float triangleHeight = texCoord.X;
		Vector3 terrainCol = Colours[0];

		for (int i = 0; i < Heights.Length; i++)
		{
			if (triangleHeight > Heights[i]) terrainCol = Colours[i + 1];
			else break;
		}

		// Calculate lighting
		float lightIntensity = (Vector3.Dot(Vector3.Normalize(normal), DirectionToLight) + 1) * 0.5f;
		terrainCol *= lightIntensity;

		// Fade to sky colour in the distance using exponential falloff
		const float atmosphereDensity = 0.0075f;
		float aerialPerspectiveT = 1 - Exp(-depth * atmosphereDensity);
		return Vector3.Lerp(terrainCol, SkyCol, aerialPerspectiveT);
	}
}
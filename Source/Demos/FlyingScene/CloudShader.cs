using System.Numerics;
using System.Runtime.CompilerServices;
using SoftwareRasterizer.Types;

namespace SoftwareRasterizer.Demo;

public class CloudShader(Vector3 directionToLight, Vector3 tint) : Shader
{
	public Vector3 DirectionToLight = directionToLight;
	public Vector3 Tint = tint;
	public Vector3 AtmosCol;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Vector3 PixelColour(Vector2 pixelCoord, Vector2 texCoord, Vector3 normal, float depth)
	{
		normal = Vector3.Normalize(normal);
		float lightIntensity = (Vector3.Dot(normal, DirectionToLight) + 1) * 0.5f;
		lightIntensity = Maths.Lerp(0.8f, 1, lightIntensity);

		float t = 1 - MathF.Exp(-depth * 0.0075f);
		Vector3 col = Vector3.Lerp(Tint * lightIntensity, AtmosCol, t);
		return col;
	}
}
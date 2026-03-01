using System.Numerics;
using System.Runtime.CompilerServices;
using SoftwareRasterizer.Types;

namespace SoftwareRasterizer.Shaders;

public class LitShader(Vector3 directionToLight, Vector3 tint) : Shader
{
	public Vector3 DirectionToLight = directionToLight;
	public Vector3 Tint = tint;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Vector3 PixelColour(Vector2 pixelCoord,Vector2 texCoord, Vector3 normal, float depth)
	{
		normal = Vector3.Normalize(normal);
		float lightIntensity = (Vector3.Dot(normal, DirectionToLight) + 1) * 0.5f;
		lightIntensity = Maths.Lerp(0.1f,1,lightIntensity);
		return Tint * lightIntensity;
	}
}
using System.Numerics;
using System.Runtime.CompilerServices;
using SoftwareRasterizer.Types;

namespace SoftwareRasterizer.Shaders;

public class LitTextureShader(Vector3 directionToLight, Texture texture) : Shader
{
	public Vector3 DirectionToLight = directionToLight;
	public Texture Texture = texture;
	public float TextureScale = 1;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Vector3 PixelColour(Vector2 pixelCoord,Vector2 texCoord, Vector3 normal, float depth)
	{
		normal = Vector3.Normalize(normal);
		float lightIntensity = (Vector3.Dot(normal, DirectionToLight) + 1) * 0.5f;
		lightIntensity = Maths.Lerp(0.4f,1,lightIntensity);
		return Texture.Sample(texCoord.X * TextureScale, texCoord.Y * TextureScale) * lightIntensity;
	}
}
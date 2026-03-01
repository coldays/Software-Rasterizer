using SoftwareRasterizer.Types;
using System.Numerics;

namespace SoftwareRasterizer.Shaders;

public class TextureShader(Texture texture) : Shader
{
	public Texture Texture = texture;

	public override Vector3 PixelColour(Vector2 pixelCoord,Vector2 texCoord, Vector3 normal, float depth) => Texture.Sample(texCoord.X, texCoord.Y);
}

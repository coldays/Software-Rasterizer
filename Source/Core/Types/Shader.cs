using System.Numerics;

namespace SoftwareRasterizer.Types;

public abstract class Shader
{
	public abstract Vector3 PixelColour(Vector2 pixelCoord, Vector2 texCoord, Vector3 normal, float depth);
}
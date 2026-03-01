using System.Numerics;

namespace SoftwareRasterizer.Types;

public class RenderTarget(int w, int h)
{
	public readonly Vector3[] ColourBuffer = new Vector3[w * h];
	public readonly float[] DepthBuffer = new float[w * h];
	public readonly object[] locks = new object[w * h];

	public readonly int Width = w;
	public readonly int Height = h;
	public readonly Vector2 Size = new(w, h);

	public void Clear(Vector3 bgCol)
	{
		for (int i = 0; i < ColourBuffer.Length; i++)
		{
			ColourBuffer[i] = bgCol;
		}

		for (int i = 0; i < DepthBuffer.Length; i++)
		{
			DepthBuffer[i] = float.PositiveInfinity;
		}

		if (locks[0] == null)
		{
			for (int i = 0; i < locks.Length; i++)
			{
				locks[i] = new object();
			}
		}
	}
}
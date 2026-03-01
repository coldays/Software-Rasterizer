using SoftwareRasterizer.Types;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SoftwareRasterizer;

public static class Maths
{
	public const float DegreesToRadians = MathF.PI / 180;
	
	// Test if point p is inside triangle ABC
	// Note: non-clockwise triangles are considered 'back-faces' and are ignored
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool PointInTriangle(in Vector2 a, in Vector2 b, in Vector2 c, in Vector2 p, out float weightA, out float weightB, out float weightC)
	{
        /// Test if point is on right side of each edge segment
        //float areaABP = SignedParallelogramArea(a, b, p);
        //float areaBCP = SignedParallelogramArea(b, c, p);
        //float areaCAP = SignedParallelogramArea(c, a, p);
        float areaABP, areaBCP, areaCAP;

        if ((areaABP = SignedParallelogramArea(a, b, p)) < 0
            || (areaBCP = SignedParallelogramArea(b, c, p)) < 0
            || (areaCAP = SignedParallelogramArea(c, a, p)) < 0)
        {
            weightA = 0;
            weightB = 0;
            weightC = 0;
            return false;
        }

        // Weighting factors (barycentric coordinates)
        float totalArea = (areaABP + areaBCP + areaCAP);
        if (totalArea <= 0)
        {
            weightA = 0;
            weightB = 0;
            weightC = 0;
            return false;
        }
        float invAreaSum = 1 / totalArea;
        weightA = areaBCP * invAreaSum;
        weightB = areaCAP * invAreaSum;
        weightC = areaABP * invAreaSum;

        return true;
    }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float SignedParallelogramArea(in Vector2 a, in Vector2 b, in Vector2 c)
	{
		return (c.X - a.X) * (b.Y - a.Y) + (c.Y - a.Y) * (a.X - b.X);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Lerp(float a, float b, float t) => a + (b - a) * Clamp01(t);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Remap01(float value, float min, float max) => Clamp01((value - min) / (max - min));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Clamp01(float value) => Math.Clamp(value, 0, 1);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Clamp(float value, float min, float max) => Math.Clamp(value, min, max);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int RoundToInt(float value) => (int)Math.Round(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ToRadians(float deg) => deg * DegreesToRadians;
	
}
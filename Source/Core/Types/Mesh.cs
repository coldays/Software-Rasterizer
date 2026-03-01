using System.Numerics;

namespace SoftwareRasterizer.Types;

public class Mesh(Vector3[] vertices, Vector3[] normals, Vector2[] texCoords)
{
	public readonly Vector3[] Vertices = vertices;
	public readonly Vector3[] Normals = normals;
	public readonly Vector2[] TexCoords = texCoords;
}
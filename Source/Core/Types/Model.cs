using SoftwareRasterizer.Types;
using System.Numerics;

namespace SoftwareRasterizer;

public class Model
{
    public Model(Mesh mesh, Shader shader, string name = "Unnamed")
    {
        Name = name;
        Mesh = mesh;
        Shader = shader;
        Vertices = mesh.Vertices;
        Normals = mesh.Normals;
        TexCoords = mesh.TexCoords;
        RasterizerPoints = new Rasterizer.RasterizerPoint[6 * mesh.Vertices.Length];
        RasterizerPointsCount = 0;
    }

    public readonly string Name;
    public readonly Mesh Mesh;
    public readonly Transform Transform = new();
    public readonly Shader Shader;
    public readonly Rasterizer.RasterizerPoint[] RasterizerPoints;
    public int RasterizerPointsCount;

    public Vector3[] Vertices;
    public Vector3[] Normals;
    public Vector2[] TexCoords;
}
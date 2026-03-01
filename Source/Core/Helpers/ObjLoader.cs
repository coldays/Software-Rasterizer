using SoftwareRasterizer.Types;
using System.Globalization;
using System.Numerics;

namespace SoftwareRasterizer.Helpers;

public static class ObjLoader
{
	static readonly string[] newLineStrings = ["\r\n", "\r", "\n"];
	static readonly CultureInfo culture = CultureInfo.InvariantCulture;

	// Highly inefficient and incomplete obj parser
	public static Mesh Load(string modelString)
	{
		string[] lines = SplitByLine(modelString, true);

		List<Vector3> vertexPositions = new();
		List<Vector3> normals = new();
		List<Vector2> texCoords = new();
		List<VertexData> allVertexData = new();

		foreach (string line in lines)
		{
			// ---- Vertex position ----
			if (line.StartsWith("v "))
			{
				string[] axisStrings = line[2..].Split(' ');
				Vector3 v = new(float.Parse(axisStrings[0], culture), float.Parse(axisStrings[1], culture), float.Parse(axisStrings[2], culture));
				vertexPositions.Add(v);
			}
			// ---- Vertex normal ----
			else if (line.StartsWith("vn "))
			{
				string[] axisStrings = line[3..].Split(' ');
				Vector3 v = new(float.Parse(axisStrings[0], culture), float.Parse(axisStrings[1], culture), float.Parse(axisStrings[2], culture));
				normals.Add(v);
			}
			else if (line.StartsWith("vt "))
			{
				string[] axisStrings = line[3..].Split(' ');
				Vector2 t = new(float.Parse(axisStrings[0], culture), float.Parse(axisStrings[1], culture));
				texCoords.Add(t);
			}
			// ---- Face Indices ----
			else if (line.StartsWith("f "))
			{
				string[] faceGroupStrings = line[2..].Split(' ');
				for (int i = 0; i < faceGroupStrings.Length; i++)
				{
					int vertexIndex = 0, tCoordIndex = 0, normalIndex = 0;
					string[] faceEntryStrings = faceGroupStrings[i].Split('/');
					bool hasVertIndex = faceEntryStrings.Length > 0 && int.TryParse(faceEntryStrings[0], culture, out vertexIndex);
					bool hasTexIndex = faceEntryStrings.Length > 1 && int.TryParse(faceEntryStrings[1], culture, out tCoordIndex);
					bool hasNormalIndex = faceEntryStrings.Length > 2 && int.TryParse(faceEntryStrings[2], culture, out normalIndex);

					VertexData vert = new()
					{
						Position = hasVertIndex ? vertexPositions[vertexIndex - 1] : Vector3.Zero,
						Normal = hasNormalIndex ? normals[normalIndex - 1] : Vector3.Zero,
						TexCoord = hasTexIndex ? texCoords[tCoordIndex - 1] : Vector2.Zero,
					};
					if (i >= 3)
					{
						allVertexData.Add(allVertexData[^(3 * i - 6)]);
						allVertexData.Add(allVertexData[^2]);
					}

					allVertexData.Add(vert);
				}
			}
		}

		Mesh mesh = new(allVertexData.Select(v => v.Position).ToArray(), allVertexData.Select(v => v.Normal).ToArray(), allVertexData.Select(v => v.TexCoord).ToArray());
		return mesh;
	}


	struct VertexData
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector2 TexCoord;
	}

	static string[] SplitByLine(string text, bool removeEmptyEntries = true)
	{
		StringSplitOptions options = removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
		return text.Split(newLineStrings, options);
	}
}

using SoftwareRasterizer.Shaders;
using SoftwareRasterizer.Types;
using System.Numerics;

namespace SoftwareRasterizer.Helpers;

public static class ResourceHelper
{
	public static Model LoadModel(string name, Shader shader = null)
	{
		string objPath = GetResourcesPath("Models", name + ".obj");
		string objString = File.ReadAllText(objPath);
		Mesh mesh = ObjLoader.Load(objString);
		Model model = new(mesh, shader ?? new LitShader(new Vector3(0, 1, 0), Vector3.One), name);
		return model;
	}

	public static Texture LoadTexture(string textureName) => Texture.CreateFromBytes(GetTextureBytes(textureName));

	public static byte[] GetTextureBytes(string textureName)
	{
		string texturePath = GetResourcesPath("Textures", textureName + ".bytes");
		return File.ReadAllBytes(texturePath);
	}

	static string GetResourcesPath(string directory, string file)
	{
		return Path.Combine(Directory.GetCurrentDirectory(), "Resources", directory, file);
	}
}
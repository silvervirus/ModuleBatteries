using System.IO;

namespace RamuneLib.Utils
{
    public static class JsonUtils
    {
        public static string GetJsonRecipe(string filename) => Path.Combine(Variables.Paths.RecipeFolder, filename + ".json");
    }
}
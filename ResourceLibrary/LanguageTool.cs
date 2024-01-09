using System.Reflection;
namespace ResourceLibrary
{
    public static class LanguageTool
    {
        public static string GetResourceValue(string resourceName)
        {
            var result = string.Empty;
            var resource = typeof(Resource);
            var property = resource.GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static);
            if (null != property)
            {
                result = property.GetValue(null) as string;
            }

            return result;
        }
    }
}

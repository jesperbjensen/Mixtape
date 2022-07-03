using System.Reflection;

namespace Mixtape;

internal class FolderStructureUtil
{
    public static IEnumerable<Route> BuildRoutesFromFolderStructure(string rootPath, string currentPath, string? baseNamespace)
    {
        var hasIndexFile = Directory.GetFiles(currentPath, "index.tsx").Length > 0;

        if (hasIndexFile)
        {
            yield return BuildIndexRoute(rootPath, currentPath, baseNamespace);
        }
        
        foreach (var directory in Directory.GetDirectories(currentPath))
        {
            foreach (var child in BuildRoutesFromFolderStructure(rootPath, Path.Combine(currentPath, directory),
                         baseNamespace))
            {
                yield return child;
            }
        }
    }

    private static Route BuildIndexRoute(string rootPath, string path, string rootNamespace)
    {
        return new Route(
            Path: FolderPathToRoutePath(rootPath, path),
            Page: FolderPathToPageInstance(rootNamespace, rootPath, path)
        );
    }

    private static object? FolderPathToPageInstance(string rootNameSpace, string contentPath, string path)
    {
        var relativePath = Path.GetRelativePath(contentPath, path);

        if (relativePath == ".")
        {
            return "/";
        }

        var segments = relativePath.Split(Path.DirectorySeparatorChar);
        
        var result = new List<string>();

        foreach (var segment in segments)
        {
            var isVariable = segment.StartsWith("$");

            if (isVariable)
            {
                result.Add("_" + segment.TrimStart('$'));
            }
            else
            {
                result.Add(segment);
            }
        }
        
        var ns = rootNameSpace + "." + string.Join(".", result);
        var type = Type.GetType(ns + ".Page," + Assembly.GetEntryAssembly().FullName);
        if (type != null)
        {
            var instance = Activator.CreateInstance(type);
            return instance;
        }

        return null;
    }

    private static string FolderPathToRoutePath(string contentPath, string path)
    {
        var relativePath = Path.GetRelativePath(contentPath, path);

        if (relativePath == ".")
        {
            return "/";
        }

        var segments = relativePath.Split(Path.DirectorySeparatorChar);
        
        var result = new List<string>();

        foreach (var segment in segments)
        {
            var isVariable = segment.StartsWith("$");

            if (isVariable)
            {
                result.Add("{" + segment.TrimStart('$') + "}");
            }
            else
            {
                result.Add(segment);
            }
        }
        
        return string.Join("/", result) + "/";

    }
}
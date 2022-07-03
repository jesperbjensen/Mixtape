using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Mixtape;

public static class WebApplicationExtensions
{
    public static void MapMixtape(this WebApplication app, string baseNamespace) {
        
        MapMixtape(app, baseNamespace, Path.Combine(app.Environment.ContentRootPath, "Pages"));
    }

    private static void MapMixtape(WebApplication app, string? baseNamespace, string contentPath)
    {
        var pages = FolderStructureUtil.BuildRoutesFromFolderStructure(contentPath, contentPath, baseNamespace);

        app.MapGet("/debug", () => JsonSerializer.Serialize(pages));

        foreach (var page in pages)
        {
            app.MapGet(page.Path, (HttpRequest request) => new
            {
                PageInfo = page,
                Variables = request.RouteValues
            });
        }
    }
}
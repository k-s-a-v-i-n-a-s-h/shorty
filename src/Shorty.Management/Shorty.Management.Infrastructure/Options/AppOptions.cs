using System.ComponentModel.DataAnnotations;

namespace Shorty.Management.Infrastructure.Options;

public sealed class AppOptions
{
    [Required, Url]
    public required string FrontendUrl { get; init; }

    [Required, Url]
    public required string BackendUrl { get; set; }

    public string GetFrontendUrl(string path) =>
        $"{FrontendUrl.TrimEnd('/')}/{path.TrimStart('/')}";

    public string GetBackendUrl(string path) =>
        $"{BackendUrl.TrimEnd('/')}/{path.TrimStart('/')}";
}

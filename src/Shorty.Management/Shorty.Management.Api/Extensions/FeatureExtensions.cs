using FluentValidation;

namespace Shorty.Management.Api.Extensions;

internal static class FeatureExtensions
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.AddValidator();

        foreach (var feature in _features)
            feature.AddServices(services);

        return services;
    }

    public static void MapFeatures(this IEndpointRouteBuilder app)
    {
        foreach (var feature in _features)
            feature.MapEndpoints(app);
    }

    private static void AddValidator(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IFeature>(includeInternalTypes: true);
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
    }

    private static readonly List<IFeature> _features = [.. typeof(IFeature).Assembly
        .GetTypes()
        .Where(p => !p.IsAbstract && typeof(IFeature).IsAssignableFrom(p))
        .Select(Activator.CreateInstance)
        .Cast<IFeature>()];
}

internal interface IFeature
{
    void AddServices(IServiceCollection services);
    void MapEndpoints(IEndpointRouteBuilder app);
}
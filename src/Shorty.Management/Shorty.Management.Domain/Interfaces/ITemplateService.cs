using Shorty.Management.Domain.Enums;

namespace Shorty.Management.Domain.Interfaces;

public interface ITemplateService
{
    Task<string> GetTemplateAsync(TemplateType type);
}

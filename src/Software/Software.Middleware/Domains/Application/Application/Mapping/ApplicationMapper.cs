using Riok.Mapperly.Abstractions;
using Software.Middleware.Domains.Application.Domain.Models.Dto.Application;
using Software.Middleware.Domains.Application.Domain.Models.Dto.ApplicationScope;
using Software.Middleware.Domains.Application.Domain.Models.Entities;

namespace Software.Middleware.Domains.Application.Application.Mapping;

[Mapper]
public partial class ApplicationMapper : IApplicationMapper
{
    [MapperIgnoreSource(nameof(ApplicationEntity.AuthId))]
    [MapperIgnoreSource(nameof(ApplicationEntity.AuthSecret))]
    [MapperIgnoreSource(nameof(ApplicationEntity.Iv))]
    public partial ApplicationGetDto ToDto(ApplicationEntity entity);

    public partial ApplicationScopeGetDto ToDto(ApplicationScopeEntity entity);
}

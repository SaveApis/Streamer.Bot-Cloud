using Software.Middleware.Domains.Application.Domain.Models.Dto.Application;
using Software.Middleware.Domains.Application.Domain.Models.Dto.ApplicationScope;
using Software.Middleware.Domains.Application.Domain.Models.Entities;

namespace Software.Middleware.Domains.Application.Application.Mapping;

public interface IApplicationMapper
{
    ApplicationGetDto ToDto(ApplicationEntity entity);
    ApplicationScopeGetDto ToDto(ApplicationScopeEntity entity);
}

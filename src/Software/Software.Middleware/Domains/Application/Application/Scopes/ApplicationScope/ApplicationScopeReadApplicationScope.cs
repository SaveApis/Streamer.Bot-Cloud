using Software.Middleware.Domains.Application.Infrastructure.Scopes;
using Software.Middleware.Domains.Common.Application.Constants;

namespace Software.Middleware.Domains.Application.Application.Scopes.ApplicationScope;

public class ApplicationScopeReadApplicationScope() : BaseApplicationScope(ApplicationScopeKeys.Application.Scope.Read, "ApplicationScope Read", "Allows reading application scopes");

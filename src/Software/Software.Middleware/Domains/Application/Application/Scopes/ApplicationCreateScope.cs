using Software.Middleware.Domains.Application.Infrastructure.Scopes;
using Software.Middleware.Domains.Common.Application.Infrastructure;

namespace Software.Middleware.Domains.Application.Application.Scopes;

public class ApplicationCreateScope() : BaseApplicationScope(ApplicationScopeKeys.Application.Create, "Application create");

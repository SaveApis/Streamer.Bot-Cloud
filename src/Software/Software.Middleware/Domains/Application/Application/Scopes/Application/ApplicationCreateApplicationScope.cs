using Software.Middleware.Domains.Application.Infrastructure.Scopes;
using Software.Middleware.Domains.Common.Application.Constants;

namespace Software.Middleware.Domains.Application.Application.Scopes.Application;

public class ApplicationCreateApplicationScope() : BaseApplicationScope(ApplicationScopeKeys.Application.Create, "Application Create", "Allows creating applications");

using Software.Middleware.Domains.Application.Infrastructure.Scopes;
using Software.Middleware.Domains.Common.Application.Infrastructure;

namespace Software.Middleware.Domains.Application.Application.Scopes;

public class ApplicationUpdateScope() : BaseApplicationScope(ApplicationScopeKeys.Application.Update, "Application update");

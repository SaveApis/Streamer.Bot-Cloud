using Software.Middleware.Domains.Application.Infrastructure.Scopes;
using Software.Middleware.Domains.Common.Application.Constants;

namespace Software.Middleware.Domains.Application.Application.Scopes.Application;

public class ApplicationUpdateApplicationScope() : BaseApplicationScope(ApplicationScopeKeys.Application.Update, "Application Update", "Allows updating/changing applications");

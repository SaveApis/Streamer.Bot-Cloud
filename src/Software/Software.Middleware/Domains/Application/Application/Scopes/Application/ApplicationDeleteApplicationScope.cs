using Software.Middleware.Domains.Application.Infrastructure.Scopes;
using Software.Middleware.Domains.Common.Application.Constants;

namespace Software.Middleware.Domains.Application.Application.Scopes.Application;

public class ApplicationDeleteApplicationScope() : BaseApplicationScope(ApplicationScopeKeys.Application.Delete, "Application Delete", "Allows deleting applications");

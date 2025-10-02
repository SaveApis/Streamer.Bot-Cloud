using Software.Middleware.Domains.Application.Infrastructure.Scopes;
using Software.Middleware.Domains.Common.Application.Constants;

namespace Software.Middleware.Domains.Application.Application.Scopes.Application;

public class ApplicationReadApplicationScope() : BaseApplicationScope(ApplicationScopeKeys.Application.Read, "Application Read", "Allows reading applications");

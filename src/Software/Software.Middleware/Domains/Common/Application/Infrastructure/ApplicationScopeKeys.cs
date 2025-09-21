namespace Software.Middleware.Domains.Common.Application.Infrastructure;

public static class ApplicationScopeKeys
{
    public static class Application
    {
        public const string Read = "application:read";
        public const string Create = "application:create";
        public const string Update = "application:update";
        public const string Delete = "application:delete";
    }
}

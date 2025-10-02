namespace Software.Middleware.Domains.Application.Domain.Options;

public class ApplicationOptionsList : List<ApplicationOption>
{
    public ApplicationOption this[string key]
    {
        get
        {
            var option = this.SingleOrDefault(x => x.Key == key);

            return option ?? throw new KeyNotFoundException($"Application option with key '{key}' not found.");

        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Utils.EntityFrameworkCore.Application.Extensions;

public static class MySqlDbContextOptionsBuilderExtensions
{
    public static void RegisterDefaultSettings(this MySqlDbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableRetryOnFailure();
        optionsBuilder.EnableIndexOptimizedBooleanColumns();
        optionsBuilder.EnableStringComparisonTranslations();
        optionsBuilder.EnablePrimitiveCollectionsSupport();
        optionsBuilder.SchemaBehavior(MySqlSchemaBehavior.Translate, (schema, name) => $"{schema}_{name}");
        optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        optionsBuilder.UseRelationalNulls();
    }
}

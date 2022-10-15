using Microsoft.EntityFrameworkCore.Query;

namespace DynamicSchema;

public class CustomQueryTranslationPreprocessorFactory : IQueryTranslationPreprocessorFactory
{
    private readonly QueryTranslationPreprocessorDependencies _dependencies;
    private readonly RelationalQueryTranslationPreprocessorDependencies _relationalDependencies;

    public CustomQueryTranslationPreprocessorFactory(QueryTranslationPreprocessorDependencies dependencies, RelationalQueryTranslationPreprocessorDependencies relationalDependencies)
    {
        _dependencies = dependencies;
        _relationalDependencies = relationalDependencies;
    }

    public QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
        => new CustomQueryTranslationPreprocessor(_dependencies, _relationalDependencies, queryCompilationContext);
}

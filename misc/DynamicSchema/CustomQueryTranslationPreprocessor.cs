using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace DynamicSchema;

public class CustomQueryTranslationPreprocessor : RelationalQueryTranslationPreprocessor
{
    public CustomQueryTranslationPreprocessor(QueryTranslationPreprocessorDependencies dependencies, RelationalQueryTranslationPreprocessorDependencies relationalDependencies, QueryCompilationContext queryCompilationContext)
        : base(dependencies, relationalDependencies, queryCompilationContext)
    {
    }

    public override Expression Process(Expression query)
        => base.Process(Preprocess(query));

    private Expression Preprocess(Expression query)
        => new FileMetaVisitor().Visit(query);
}

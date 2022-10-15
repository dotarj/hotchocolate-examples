using System.Linq.Expressions;

namespace DynamicSchema;

public class FileMetaVisitor : ExpressionVisitor
{
    protected override Expression VisitConditional(ConditionalExpression node)
    {
        if (node.Test is BinaryExpression { Left: MemberExpression memberExpression }
            && memberExpression.Type == typeof(List<FileMeta>))
        {
            var selectExpression = Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.Select),
                new[]
                {
                    typeof(FileMeta),
                    typeof(FileMeta)
                },
                memberExpression,
                CreateMemberInitLambda());

            return ToList(typeof(FileMeta), selectExpression);
        }

        return base.VisitConditional(node);
    }

    private static Expression ToList(Type returnType, Expression source)
    {
        return Expression.Call(
            typeof(Enumerable),
            nameof(Enumerable.ToList),
            new[]
            {
                returnType
            },
            source);
    }

    private Expression CreateMemberInitLambda()
        => (FileMeta fileMeta) => new FileMeta
        {
            FileMetaId = fileMeta.FileMetaId,
            FileId = fileMeta.FileId,
            FileMetaTypeId = fileMeta.FileMetaTypeId,
            Value = fileMeta.Value,
            Type = fileMeta.Type != null ? new FileMetaType
            {
                FileMetaTypeId = fileMeta.Type.FileMetaTypeId,
                FileTypeId = fileMeta.Type.FileTypeId,
                Key = fileMeta.Type.Key,
                DataType = fileMeta.Type.DataType
            } : null
        };
}

using System.ComponentModel;
using HotChocolate.Resolvers;

namespace DynamicSchema;

public class FileMetaResolver
{
    public T? GetMetaValue<T>([Parent] List<FileMeta> metas, IResolverContext context)
    {
        var key = context.Selection.SyntaxNode.ToString();
        var meta = metas.FirstOrDefault(meta => meta.Type.Key == key);

        if (meta == null)
        {
            return default;
        }
        
        var typeConverter = TypeDescriptor.GetConverter(typeof(T));

        return (T?)typeConverter.ConvertFromString(meta.Value);
    }
}

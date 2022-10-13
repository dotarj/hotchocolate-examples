using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Descriptors.Definitions;

namespace DynamicSchema;

public class FileMetaTypeModule : ITypeModule
{
    private readonly FilesDbContext _dbContext;

    public FileMetaTypeModule(FilesDbContext dbContext)
        => _dbContext = dbContext;

    public ValueTask<IReadOnlyCollection<ITypeSystemMember>> CreateTypesAsync(IDescriptorContext context, CancellationToken cancellationToken)
    {
        var metasType = CreateMetasType(context, _dbContext);
        var fileType = CreateFileType(metasType);

        return ValueTask.FromResult((IReadOnlyCollection<ITypeSystemMember>)new [] { metasType, fileType });
    }

    private static ObjectType CreateMetasType(IDescriptorContext context, FilesDbContext dbContext)
    {
        var metasTypeDefinition = new ObjectTypeDefinition("metas");
        var fieldDefinitions = CreateMetasTypeFieldDefinitions(context, dbContext);

        metasTypeDefinition.Fields.AddRange(fieldDefinitions);

        return ObjectType.CreateUnsafe(metasTypeDefinition);
    }

    private static IEnumerable<ObjectFieldDefinition> CreateMetasTypeFieldDefinitions(IDescriptorContext context, FilesDbContext dbContext)
        => dbContext.FileTypes.SelectMany(fileType => fileType.MetaTypes)
            .Select(fileMetaType => CreateField(context, fileMetaType));

    private static ObjectFieldDefinition CreateField(IDescriptorContext context, FileMetaType fileTypeMeta)
    {
        var field = new ObjectFieldDefinition(fileTypeMeta.Key, type: TypeReference.Parse(fileTypeMeta.DataType.ToString()));
        var fieldDescriptor = ObjectFieldDescriptor.From(context, field);

        SetFieldResolver(fieldDescriptor, fileTypeMeta);

        fieldDescriptor.CreateDefinition();

        return field;
    }

    private static void SetFieldResolver(ObjectFieldDescriptor fieldDescriptor, FileMetaType fileTypeMeta)
    {
        switch (fileTypeMeta.DataType)
        {
            case FileMetaDataType.String:
                fieldDescriptor.ResolveWith<FileMetaResolver>(resolver => resolver.GetMetaValue<string>(default, default));
                break;
            case FileMetaDataType.Int:
                fieldDescriptor.ResolveWith<FileMetaResolver>(resolver => resolver.GetMetaValue<int>(default, default));
                break;
            case FileMetaDataType.DateTime:
                fieldDescriptor.ResolveWith<FileMetaResolver>(resolver => resolver.GetMetaValue<DateTime>(default, default));
                break;
        }
    }

    private static ObjectType<File> CreateFileType(ObjectType metasType)
    {
        return new ObjectType<File>(descriptor =>
        {
            descriptor.BindFieldsExplicitly();
            descriptor.Field(t => t.FileId);
            descriptor.Field(t => t.Name);
            descriptor.Field(t => t.Metas)
                .Extend()
                .Definition.Type = TypeReference.Create(new NonNullType(metasType));
        });
    }

    public event EventHandler<EventArgs>? TypesChanged;
}

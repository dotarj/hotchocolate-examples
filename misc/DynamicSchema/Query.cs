namespace DynamicSchema;

public class Query
{
    [UseProjection]
    public IQueryable<File> GetFiles([Service] FilesDbContext dbContext)
        => dbContext.Files;
}

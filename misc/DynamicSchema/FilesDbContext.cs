using Microsoft.EntityFrameworkCore;

namespace DynamicSchema;

public class FilesDbContext : DbContext
{
    public FilesDbContext(DbContextOptions<FilesDbContext> options)
        : base(options)
    {
    }

    public DbSet<File> Files { get; set; } = null!;

    public DbSet<FileType> FileTypes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<File>(entity =>
        {
            entity.HasMany(file => file.Metas)
                .WithOne()
                .HasForeignKey(fileMeta => fileMeta.FileId);
            
            entity
                .HasData(new File { FileId = 1, FileTypeId = 1, Name = "Grunn.png" });
        });

        modelBuilder.Entity<FileType>(entity =>
        {
            entity.HasMany(fileType => fileType.MetaTypes)
                .WithOne()
                .HasForeignKey(fileMetaType => fileMetaType.FileTypeId);
            
            entity
                .HasData(new FileType { FileTypeId = 1, Name = "Image" });
        });

        modelBuilder.Entity<FileMetaType>(entity =>
        {
            entity
                .HasData(
                    new FileMetaType { FileMetaTypeId = 1, FileTypeId = 1, Key = "width", DateType = FileMetaDataType.Int },
                    new FileMetaType { FileMetaTypeId = 2, FileTypeId = 1, Key = "height", DateType = FileMetaDataType.Int },
                    new FileMetaType { FileMetaTypeId = 3, FileTypeId = 1, Key = "dateTaken", DateType = FileMetaDataType.DateTime },
                    new FileMetaType { FileMetaTypeId = 4, FileTypeId = 1, Key = "location", DateType = FileMetaDataType.String });
        });

        modelBuilder.Entity<FileMeta>(entity =>
        {
            entity.HasOne(fileMeta => fileMeta.Type)
                .WithMany()
                .HasForeignKey(fileMeta => fileMeta.FileMetaTypeId);

            entity
                .HasData(
                    new FileMeta { FileMetaId = 1, FileId = 1, FileMetaTypeId = 1, Value = "1024" },
                    new FileMeta { FileMetaId = 2, FileId = 1, FileMetaTypeId = 2, Value = "768" },
                    new FileMeta { FileMetaId = 3, FileId = 1, FileMetaTypeId = 3, Value = "2022-01-01 00:00:00" },
                    new FileMeta { FileMetaId = 4, FileId = 1, FileMetaTypeId = 4, Value = "53°13'07.4\"N 6°34'04.1\"E" });
        });
    }
}

public class File
{
    public int FileId { get; set; }

    public string Name { get; set; } = string.Empty;

    [GraphQLIgnore]
    public int FileTypeId { get; set; }

    public FileType Type { get; set; } = null!;

    public ICollection<FileMeta> Metas { get; set; } = new List<FileMeta>();
}

public class FileType
{
    public int FileTypeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<FileMetaType> MetaTypes { get; set; } = new List<FileMetaType>();
}

public class FileMetaType
{
    public int FileMetaTypeId { get; set; }

    [GraphQLIgnore]
    public int FileTypeId { get; set; }

    public string Key { get; set; } = string.Empty;

    public FileMetaDataType DateType { get; set; }
}

public class FileMeta
{
    public int FileMetaId { get; set; }

    [GraphQLIgnore]
    public int FileId { get; set; }

    [GraphQLIgnore]
    public int FileMetaTypeId { get; set; }

    public string Value { get; set; } = string.Empty;

    public FileMetaType Type { get; set; } = null!;
}

public enum FileMetaDataType
{
    String,
    Int,
    DateTime
}
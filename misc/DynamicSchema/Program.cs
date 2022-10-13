using DynamicSchema;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FilesDbContext>(options =>
{
    var folder = Environment.SpecialFolder.LocalApplicationData;
    var path = Environment.GetFolderPath(folder);
    
    options.UseSqlite($"Data Source={System.IO.Path.Join(path, "files.db")}");
});

builder.Services
    .AddGraphQLServer()
    .AddProjections()
    .RegisterDbContext<FilesDbContext>()
    .AddQueryType<Query>()
    .AddTypeModule(serviceProvider => new FileMetaTypeModule(serviceProvider.GetRequiredService<FilesDbContext>()));

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(endpoints => endpoints.MapGraphQL());

app.Run();

# Getting started

Follow these steps to run the sample:

1. Install the Entity Framework Core .NET Core CLI tools `dotnet tool install --global dotnet-ef`
1. Create a migration script: `dotnet ef migrations add InitialCreate`
1. Create the SQLite database: `dotnet ef database update`
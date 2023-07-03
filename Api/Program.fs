namespace Api
#nowarn "20" // Disable FS0020: This expression should have type 'unit', but has type 'WebApplicationBuilder'
open System
open Data.Repository
open Data.Todo
open Domain.Repositories
open Domain.Services
open Microsoft.AspNetCore.Builder
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Infrastructure
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open NSwag
open NSwag.Generation.AspNetCore
open EntityFrameworkCore.FSharp.Extensions


module Program =
    [<Literal>]
    let ExitCode = 0
    
    let private sqliteConfigure (builder: SqliteDbContextOptionsBuilder) =
        builder.MigrationsAssembly("Api") |> ignore
    
    let private dbConfigure (connectionString: string) (options: DbContextOptionsBuilder) =
        options
            .UseSqlite(connectionString, sqliteConfigure)
            .UseFSharpTypes() |> ignore
        
    let swaggerDocument (document: OpenApiDocument) =
        document.Info.Title <- "Todo API"
        document.Info.Version <- "v1"
        document.Info.Description <- "A simple example ASP.NET Core Web API"
        document.Info.TermsOfService <- "None"
        document.Info.Contact <- OpenApiContact(Name = "Fernando", Email = "fwfurtado@gmail.com")
    let swagger (config: AspNetCoreOpenApiDocumentGeneratorSettings) =
        config.PostProcess <- swaggerDocument

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)
        let connectionString = builder.Configuration.GetConnectionString("default")

        builder.Services.AddControllers()
        builder.Services.AddSwaggerDocument(swagger)
        builder.Services.AddDbContext<Context>(dbConfigure connectionString)
        builder.Services.AddScoped<ITodoRepository, TodoRepository>()
        builder.Services.AddTransient<TodoServices>()

        let app = builder.Build()

        app.UseHttpsRedirection()

        app.UseAuthorization()
        app.MapControllers()
        app.UseOpenApi()
        app.UseSwaggerUi3()

        app.Run()

        ExitCode

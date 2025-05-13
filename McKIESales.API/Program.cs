using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using McKIESales.API;
using McKIESales.API.Models;
using McKIESales.API.Services;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<IMongoClient>(sserviceProvider => {
    var connectionString = builder.Configuration.GetSection("MongoDB")["ConnectionString"];
    return new MongoClient(connectionString);
});
builder.Services.AddSingleton<ShopContext>();

builder.Services.AddControllers();

//  This code configures API versioning for a .NET application. It sets the default API version to 1.0
//  and assumes this version when the client does not specify one. It also enables reporting of the API
//  version in responses. The `ApiVersionReader` is configured to read the API version from the header
//  (`x-api-version`), query string (`x-api-version`), or URL segment.
//  Additionally, it configures the MVC to use API versioning with custom version formatting in the URL,
//  ensures the version is substituted in the URL, and adds version parameters for neutral endpoints
//  (those without a specific version).
builder.Services.AddApiVersioning(options => {
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new HeaderApiVersionReader("x-api-version"),
        new QueryStringApiVersionReader("x-api-version"),
        new UrlSegmentApiVersionReader()
    );
}).AddMvc().AddApiExplorer(options => {
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
    options.AddApiVersionParametersWhenVersionNeutral = true;
});

builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

var apiProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

//  In the development environment, the code enables Swagger and configures the Swagger UI to
//  display API documentation for each version. It retrieves the API version descriptions from
//  the `IApiVersionDescriptionProvider` and dynamically sets up a Swagger endpoint for each version.
//  In production, it disables Swagger and enables HSTS (HTTP Strict Transport Security) to enforce
//  secure HTTPS connections.
if (app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI(options => { 
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions){
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
} else {
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//  This code creates a scoped service provider to access the application's dependencies
//  within a specific scope. It retrieves an `IMongoClient` from the service provider,
//  uses it to access the `bowling_supplies` database, and then creates an instance of
//  `DatabaseSeeder` to populate the database with initial data. The seeding process is
//  executed asynchronously by calling the `SeedAsync` method.
using (var scope = app.Services.CreateScope()){
    var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();
    var database = mongoClient.GetDatabase("bowling_supplies");

    var seeder = new DatabaseSeeder(database);
    await seeder.SeedAsync();
}

app.Run();
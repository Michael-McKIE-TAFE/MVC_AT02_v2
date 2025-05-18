using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning.Routing;
using McKIESales.API;
using McKIESales.API.Models;
using McKIESales.API.Services;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

//  Configures `MongoDBSettings` from the app's configuration,
//  making it accessible for dependency injection throughout the application.
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));

//  Registers `IMongoClient` as a singleton service, using the connection
//  string from the configuration to create a `MongoClient` instance.
builder.Services.AddSingleton<IMongoClient>(sserviceProvider => {
    var connectionString = builder.Configuration.GetSection("MongoDB")["ConnectionString"];
    return new MongoClient(connectionString);
});

//  Registers `ShopContext` as a singleton service, allowing it to be
//  injected into other components throughout the application.
builder.Services.AddSingleton<ShopContext>();

builder.Services.AddControllers();

//  Configures API versioning. It sets the default API version to 1.0
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

//  Configures Swagger options by using the `ConfigureSwaggerOptions` class.
//  It registers it so that Swagger UI is customized when the application runs.
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

//  Configures route options by adding a custom route constraint
//  (`ApiVersionRouteConstraint`) for API versioning. It ensures
//  that routes can be validated based on the API version.
builder.Services.Configure<RouteOptions>(options => {
    options.ConstraintMap.Add("apiVersion", typeof(ApiVersionRouteConstraint));
});

//  Sets up a CORS policy named "AllowAll," allowing any origin, method, and header.
//  It enables cross-origin requests from any source for the application.
builder.Services.AddCors(options => { 
    options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

//  Retrieves the required `IApiVersionDescriptionProvider` service from the application's
//  service container. It is typically used to handle API versioning and provides information
//  about available API versions.
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

app.UseCors();

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
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using McKIESales.API;
using McKIESales.API.Models;
using McKIESales.API.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<IMongoClient>(sserviceProvider => {
    var connectionString = builder.Configuration.GetSection("MongoDB")["ConnectionString"];
    return new MongoClient(connectionString);
});
builder.Services.AddSingleton<ShopContext>();

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options => {
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new ApiVersion(1,0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new HeaderApiVersionReader("X-API-version");
});

builder.Services.AddVersionedApiExplorer(options => {
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var apiProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI(options => { 
        foreach (var description in apiProvider.ApiVersionDescriptions){
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"My API {description.GroupName.ToUpperInvariant()}");
        }
    });
} else {
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope()){
    var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();
    var database = mongoClient.GetDatabase("bowling_supplies");

    var seeder = new DatabaseSeeder(database);
    await seeder.SeedAsync();
}

app.Run();
using McKIESales.API.Models;
using McKIESales.API.Controllers;
using McKIESales.API.Services;
using MongoDB.Driver;
using Microsoft.OpenApi.Writers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<IMongoClient>(sserviceProvider => {
    var connectionString = builder.Configuration.GetSection("MongoDB")["ConnectionString"];
    return new MongoClient(connectionString);
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ShopContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
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
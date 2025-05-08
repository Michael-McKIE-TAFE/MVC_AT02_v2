using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApiVersioning();
builder.Services.AddVersionedApiExplorer(); // <-- should work

var app = builder.Build();
app.MapControllers();
app.Run();
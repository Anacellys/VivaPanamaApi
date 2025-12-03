using Microsoft.EntityFrameworkCore;
using VivaPanamaApi.Data;

var builder = WebApplication.CreateBuilder(args);
// aqui se lee el appsettings.json, se cargan variables, servicios,etc. 

// Add services to the container.

builder.Services.AddControllers();

// Configurar Entity Framework Core con PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

using Microsoft.EntityFrameworkCore;
using System;
using VivaPanamaApi.Data; // Asegúrate de que este es el namespace correcto

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// 🔹 1. Agregar conexión a PostgreSQL
// ----------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();
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
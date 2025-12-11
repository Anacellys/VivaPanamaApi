using Microsoft.EntityFrameworkCore;
using VivaPanamaApi.Data;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// 🔹 1. Conexión a PostgreSQL
// ----------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ----------------------------------------------------
// 🔹 2. CORS: permitir llamadas desde tu HTML (127.0.0.1:5500)
// ----------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()   // si quieres, luego lo cambiamos a origen específico
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Servicios MVC / Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ----------------------------------------------------
// 🔹 3. Middleware
// ----------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 👈 muy importante: CORS va *antes* de Authorization
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();

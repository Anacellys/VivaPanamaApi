var builder = WebApplication.CreateBuilder(args); // este de aqui crea el constructor del proyecto
// aqui se lee el appsettings.json, se cargan variables, servicios,etc. 

// Add services to the container.

builder.Services.AddControllers(); //esto añade controladores
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(); //Swagger es el UI donde pruebas el API

var app = builder.Build(); // Aqui se crea la aplicacion lista para atender peticiones HTTP 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // Si estas en modo desarrollo activa Swagger, activa el explorador de la API
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); //Fuerza que las peticiones sean HTTPS

app.UseAuthorization(); // Habilita la autorización (aunque aún no tienes auth configurado)

app.MapControllers(); //Conecta todas las clases Controller.

app.Run(); //Arranca el servidor 

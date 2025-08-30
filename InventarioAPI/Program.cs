using InventarioAPI.Data;
using InventarioAPI.Data.Contrato;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// INYECTAR DEPENDENCIAS
builder.Services.AddScoped<IUsuario, UsuarioRepositorio>();
builder.Services.AddScoped<IProducto, ProductoRepositorio>();
builder.Services.AddScoped<IMovimientos, MovimientoRepositorio>();

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

using System.Reflection;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Configurando CORS do front
var FrontendOrigins = "_frontendOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: FrontendOrigins,
        builder =>
        {
            builder.WithOrigins("http://localhost:5173", "http://localhost:5200")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
});

// configurando o JSON para ser case insensitive e usar camelCase
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// configurando DbContext para usar SQLite
builder.Services.AddDbContext<webapi.AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// configurando Validators
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// fazendo o build e adicionando configurações de middleware
var app = builder.Build();
app.MapControllers();
app.UseCors(FrontendOrigins);
app.UsePathBase("/api");

// rodando o app
app.Run();

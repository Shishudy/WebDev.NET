using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using LibEF;
using LibEF.Models;
using Microsoft.EntityFrameworkCore;
using WebAPI.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll",
		policy => policy.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

Model model = new Model(app);

app.MapPost("/login", (Object response) =>
{
	return model.Login(response);
})
.WithName("Login")
.WithOpenApi();

app.MapGet("/methods", () =>
{
	return (model.Methods);
}).WithName("Methods").WithOpenApi();

app.MapGet("/weatherforecast", (string str) =>
{
	ProjectoContext context = new ProjectoContext();
	EF_methods EF = new EF_methods(context);
	var forecast = EF.GetPassWordbyLogin(str);
	var obj = new { result = forecast};
	var json = JsonSerializer.Serialize(obj);
	return (json);
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.UseCors("AllowAll");

app.Run();
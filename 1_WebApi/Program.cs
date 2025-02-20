using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using LibEF;
using LibEF.Models;
using Microsoft.EntityFrameworkCore;

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

app.MapPost("/login", (Object response) =>
{
	ProjectoContext context = new ProjectoContext();
	EF_methods EF = new EF_methods(context);
	string jsonRes = JsonSerializer.Serialize(response);
	Dictionary<string, string> res = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonRes);
	try
	{
		var forecast = EF.GetPassWordbyLogin(res.GetValueOrDefault("username"));
		var obj = new { result = forecast};
		var json = JsonSerializer.Serialize(obj);
		return (json);
	}
	catch
	{
		var obj = new { result = "User not found."};
		var json = JsonSerializer.Serialize(obj);
		return (json);
	}
})
.WithName("Login")
.WithOpenApi();

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
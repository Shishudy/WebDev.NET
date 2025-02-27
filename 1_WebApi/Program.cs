using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using LibEF;
using LibEF.Models;
using Microsoft.AspNetCore.Mvc;
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
	return (model.MethodsJson);//this should only pass method names not everything
	//return (model.Methods.keys);
})
.WithName("Methods")
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

app.MapPost("/ResolveMethod/{method}", (string method, [FromBody] JsonElement param) =>
{
	try
	{
		var methods_dic = model.MethodsDict;
		var ParamList = model.GetParamList(method, param);
		//return Results.Ok(ParamList); //at this point we dio get [1]
		if (!methods_dic.ContainsKey(method))
			throw new Exception("no such method listed");
		if (ParamList != null) // parameters are recieved and the method is ran.
		{
            return Results.Ok((model.ResolveMethod(method, ParamList)));
		}
		else if (methods_dic[method] == null)
		{ // no need for parameters method is just run
			return Results.Ok((model.ResolveMethod(method, null)));
		}
		else // sends the parameters back
		{
			return Results.Ok(JsonSerializer.Serialize(methods_dic[method]));
		}
	}
	catch (Exception ex)
	{
		return Results.BadRequest(ex.Message);
	}
})
.WithName("ResolveMethod")
.WithOpenApi();


app.MapPost("/teste", (Object response) =>
{ 
	Console.WriteLine(response);
	var json = JsonSerializer.Serialize(response);
	return (json);
})
.WithName("Teste")
.WithOpenApi();

app.UseCors("AllowAll");

app.Run();
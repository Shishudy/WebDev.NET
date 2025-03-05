using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Model;
using System.Text;

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

builder.Services.AddAuthorization(); // Add this line

// Read settings from appsettings.json
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");
var jwtSettings = configuration.GetSection("JwtSettings");
Model model = new Model(connectionString);
methodsMapping map_method = new methodsMapping();

/////// ++++++++++ /////
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]); // Replace with your secret key
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = jwtSettings["Issuer"],  // Local development (http://localhost:5000) or production (https://mywebsite.com)
		ValidAudience = jwtSettings["Audience"], // the audience that the token is intended for ( the URL of the API)
		IssuerSigningKey = new SymmetricSecurityKey(key)
	};
});
/////// ++++++++++ /////

var app = builder.Build(); 

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowAll"); //
app.UseAuthentication(); // Add this line
app.UseAuthorization(); // Add this line


app.MapPost("/login", (JsonElement jsonRes) =>
{
	try
	{
		var isAuthenticated = model.Login(jsonRes);
		if (isAuthenticated)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "admin") }),
				Expires = DateTime.UtcNow.AddHours(1),
				Issuer = jwtSettings["Issuer"], // Replace with your issuer
				Audience = jwtSettings["Audience"], // Replace with your audience
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tokenString = tokenHandler.WriteToken(token);

			return Results.Ok(new { Token = tokenString });
		}
		else
			return Results.Ok("");
	}
	catch (Exception ex)
	{
		return Results.BadRequest(ex.Message);
	}
})
.WithName("Login")
.WithOpenApi();

app.MapGet("/items/{tab}/{search}/{page}", (string tab, string? search = null, string? page = null) =>
{
    try
	{
		return Results.Ok(model.GetData(tab, search, page));
	}
	catch (Exception ex)
	{
		return Results.BadRequest(ex.Message);
	}
})
.WithName("Items")
.WithOpenApi();
// .WithAuthorization();

app.MapPost("/ResolveMethod/{method}", (string method, string? page, [FromBody] JsonElement param) =>
{
	try
	{
		model.MethodCaller(method, param);
		return Results.Ok("Sucesso!");
	}
	catch (Exception ex)
	{
		return Results.BadRequest(ex.Message);
	}
})
.WithName("ResolveMethod")
.WithOpenApi();
// .WithAuthorization();


//app.MapPost("/teste", (Object response) =>
//{ 
//	Console.WriteLine(response);
//	var json = JsonSerializer.Serialize(response);
//	return (json);
//})
//.WithName("Teste")
//.WithOpenApi();

// app.MapGet("/weatherforecast", (string str) =>
// {
//    ProjectoContext context = new ProjectoContext();
//    EF_methods EF = new EF_methods(context);
//    var forecast = EF.GetPassWordbyLogin(str);
//    var obj = new { result = forecast };
//    var json = JsonSerializer.Serialize(obj);
//    return (json);
// })
// .WithName("GetWeatherForecast")
// .WithOpenApi();
// .WithAuthorization();


app.UseCors("AllowAll");
app.Run();
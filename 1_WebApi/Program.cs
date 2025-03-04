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

Model model = new Model(connectionString);

app.MapPost("/login", (JsonElement jsonRes) =>
{
	try
	{
		// Authenticate the user and generate a JWT token
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
		{
			return Results.BadRequest("Invalid login attempt.");
		}
	}
	catch (Exception ex)
	{
		return Results.BadRequest(ex.Message);
	}
})
.WithName("Login")
.WithOpenApi();

//app.MapPost("/login", (JsonElement jsonRes) =>
//{
//	try
//	{
//		return Results.Ok(model.Login(jsonRes));
//	}
//	catch (Exception ex)
//	{
//		//return Results.BadRequest(ex.Message);
//		return Results.BadRequest(false);
//	}
//})
//.WithName("Login")
//.WithOpenApi();

app.MapGet("/methods/{level}/{category}", (string level,string cat) =>
{
	try
	{
		return Results.Ok(model.GetMethods(level, cat));
	}
	catch (Exception ex)
	{
		return Results.BadRequest(ex.Message);
	}
	//return (model.Methods.keys);
})
.WithName("Methods")
.WithOpenApi();
// .WithAuthorization();

app.MapPost("/ResolveMethod/{method}/{page}/{size}", (string method, string? page, string? size, [FromBody] JsonElement param) =>
{
	try
	{
		var methods_dic = model.MethodsDict;
		var ParamList = model.GetParamList(method, param);
		if (!methods_dic.ContainsKey(method))
			throw new Exception("no such method listed");
		// retorna os parametros necessarios
		if ((ParamList == null || ParamList.Count == 0) && methods_dic[method] != null)
            return Results.Ok(JsonSerializer.Serialize(methods_dic[method]));
		// vai buscar a lista
		var result = model.ResolveMethod(method, ParamList);
		if (page == null || size == null)
			return Results.Ok(result);
		// separar pacotes a enviar
		int sizeInt = int.Parse(size);
		int pageInt = int.Parse(page) * sizeInt;
		if (pageInt < 0 || sizeInt < 0)
            throw new Exception("invalid page or size");
        var pagedResult = result as List<object>;
        if (pagedResult == null)
            throw new Exception("result is not a list");
        if (pageInt + sizeInt >= pagedResult.Count)
            throw new Exception("page out of range");
        pagedResult = pagedResult.Skip(pageInt * sizeInt).Take(sizeInt).ToList();
        return Results.Ok(pagedResult);
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
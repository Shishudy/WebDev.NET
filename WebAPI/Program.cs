var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//ARGV method
//app.MapPost("/items", (Dictionary<string, string> args) =>
//{
//    return Results.Json(args);
//});



app.MapPost("/login", (string login, string password) =>
{
    if (login == "admin" && password == "admin")
    {
        return Results.Ok("You are logged in");
    }
    else
    {
        return Results.BadRequest("Incorrect login or password");
    }
})
.WithName("Login")
.WithOpenApi();

app.Run();

using System.Reflection;
using LibADO.Login;
using Microsoft.AspNetCore.Http; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<LoginRepository>(provider =>
    new LoginRepositoryADO("Server=PC013562;Database=Projecto;Integrated Security=True;TrustServerCertificate=True;"));
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // ?? Permite acessar HttpContext no Razor

AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
{
    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LibDB.dll");
    return File.Exists(path) ? Assembly.LoadFile(path) : null;
};

var app = builder.Build();

app.UseSession();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.Run();

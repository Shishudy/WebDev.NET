using System.Reflection;
using LibADO.Login;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<LoginRepository>(provider =>
    new LoginRepositoryADO("Server=PC013562;Database=Projecto;Integrated Security=True;TrustServerCertificate=True;"));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
{
    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LibDB.dll");
    return File.Exists(path) ? Assembly.LoadFile(path) : null;
};

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthorization();
app.MapRazorPages();
app.Run();

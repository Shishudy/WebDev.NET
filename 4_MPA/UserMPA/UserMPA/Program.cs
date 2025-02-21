using System.Reflection;
using LibADO.Login;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();

builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<LoginRepository>(provider =>
    new LoginRepositoryADO("Server=PC013562;Database=Projecto;Integrated Security=True;TrustServerCertificate=True;"));


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

app.UseAuthorization();

app.MapRazorPages();

app.Run();

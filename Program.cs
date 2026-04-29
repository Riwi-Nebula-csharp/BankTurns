using BankTurns.Data;
using BankTurns.Interfaces;
using BankTurns.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddScoped<ITurnHistoryService, TurnHistoryService>();
builder.Services.AddScoped<ITurnService,        TurnService>();
builder.Services.AddScoped<IUserService,        UserService>();
builder.Services.AddScoped<IAdvisorService,     AdvisorService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Mapear también rutas de API
app.MapControllers();

app.Run();
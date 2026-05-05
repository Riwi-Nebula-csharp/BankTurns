using BankTurns.Data;
using BankTurns.Hubs;
using BankTurns.Interfaces;
using BankTurns.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Session para autenticación del asesor
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddScoped<ITurnHistoryService, TurnHistoryService>();
builder.Services.AddScoped<ITurnService,        TurnService>();
builder.Services.AddScoped<IUserService,        UserService>();
builder.Services.AddScoped<IAdvisorService,     AdvisorService>();

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ruta del Hub de SignalR
app.MapHub<TurnHub>("/turnHub");

app.Run();
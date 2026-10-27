using Gestion_Condominios.Data;
using Microsoft.EntityFrameworkCore;
using Gestion_Condominios.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CondominioDbContext>(options =>

    options.UseSqlServer(builder.Configuration.GetConnectionString("GestionCondominio"))
);
builder.Services.AddScoped<IOcupacionService, OcupacionService>();
builder.Services.AddScoped<IPagosService, PagosService>();
builder.Services.AddScoped<IGeneradorExpensasService, PagosService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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


app.Run();

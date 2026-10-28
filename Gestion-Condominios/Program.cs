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

// Carga explícita de datos de prueba; no se ejecuta al iniciar normalmente.
if (app.Configuration.GetValue<bool>("SeedDemo"))
{
    if (!app.Environment.IsDevelopment())
        throw new InvalidOperationException(
            "La carga DEMO solo está permitida en Development.");

    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<CondominioDbContext>();

    await db.Database.MigrateAsync();
    await BogusSeeder.CargarAsync(db);
    return;
}


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

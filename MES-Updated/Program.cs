using BoldReports.Web;
using f10.pulsar.mes;
using f10.pulsar.mes.data;
using f10.pulsar.sv.data;
using f10.pulsar.sv.data.mariadb;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true;}).AddHubOptions(o=>o.MaximumReceiveMessageSize = 10 * 1024 * 1024);

Bold.Licensing.BoldLicenseProvider.RegisterLicense("1cpmShorfWau1b/kdRDIVPbGs63YGuvMm0wwC4B76vQ=");

ReportConfig.DefaultSettings = new ReportSettings().RegisterExtensions(new List<string>
{
    "BoldReports.Data.WebData",
    "BoldReports.Data.Csv"
});

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzk0ODMxOEAzMzMwMmUzMDJlMzAzYjMzMzAzYkVtek9qZlB4MWY3TENQaC9xaVMwZzdnUHoyWEo5dis2TjE3YlhGcE9VNUk9");
builder.Services.AddSyncfusionBlazor();


//Common
builder.Services.AddScoped<Utils>();
builder.Services.AddScoped<ReceitaStepsDTO>();
builder.Services.AddScoped<IReportDataService, ReportDataService>();
builder.Services.AddScoped<ThemeState>();

//MES
builder.Services.AddScoped<CaracteristicasService>();
builder.Services.AddScoped<EquipamentosService>();
builder.Services.AddScoped<UserSettingsService>();

//Linha Services
builder.Services.AddScoped<Linha1Service>();
builder.Services.AddScoped<UtilizadoresService>();
builder.Services.AddScoped<PermissionsService>();
builder.Services.AddScoped<IBanhosService,BanhosService>();
builder.Services.AddScoped<IBanhoReceitaService,BanhoReceitaService>();

//Linha Repositories
builder.Services.AddScoped<IBanhoRepository, BanhoRepository>();
builder.Services.AddScoped<IBanhoReceitaRepository, BanhoReceitaRepository>();
builder.Services.AddScoped<IBanhoReceitaParametroRepository, BanhoReceitaParametroRepository>();

builder.Services.AddScoped<ITinasRepository, TinasRepository>();

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

builder.Services.AddDbContextFactory<MySqlDataContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MES"),
        new MariaDbServerVersion(new Version(11, 8, 2))
    )
);

builder.Services.AddDbContextFactory<PulsarDataContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("LINHA1"),
        new MariaDbServerVersion(new Version(11, 8, 2))
    )
);

var app = builder.Build();

//Apagar no futuro 06/02/20256
//aplica migrations automaticamente
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PulsarDataContext>();

    db.Database.EnsureCreated();

    if (db.Database.CanConnect())
        db.Database.Migrate();
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseRouting();
app.MapControllers();

var supportedCultures = new[] { "pt-PT" };
app.UseRequestLocalization(new RequestLocalizationOptions().AddSupportedCultures(supportedCultures).AddSupportedUICultures(supportedCultures).SetDefaultCulture(supportedCultures[0]));

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

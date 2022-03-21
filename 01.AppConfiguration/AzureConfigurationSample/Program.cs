using AzureConfigurationSample;
using AzureConfigurationSample.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from Azure App Configuration
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(Environment.GetEnvironmentVariable("AppConfiguration"))
        // Load all keys that start with `WebDemo:` and have no label
        //.Select("WebDemo:*")
        // Configure to reload configuration if the registered key 'WebDemo:Sentinel' is modified.
        // Use the default cache expiration of 30 seconds. It can be overriden via AzureAppConfigurationRefreshOptions.SetCacheExpiration.
        .ConfigureRefresh(refreshOptions =>
        {
            refreshOptions.Register("Settings:Sentinel", refreshAll: true).SetCacheExpiration(TimeSpan.FromSeconds(5));
        })
        // Load all feature flags with no label. To load specific feature flags and labels, set via FeatureFlagOptions.Select.
        // Use the default cache expiration of 30 seconds. It can be overriden via FeatureFlagOptions.CacheExpirationInterval.
        .UseFeatureFlags();
});
//builder.Configuration.AddAzureAppConfiguration(Environment.GetEnvironmentVariable("AppConfiguration"));

builder.Services.AddSingleton<ITargetingContextAccessor, TestTargetingContextAccessor>();
builder.Services.AddAzureAppConfiguration()
    .AddFeatureManagement()
    .AddFeatureFilter<TargetingFilter>();

builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;

        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 0;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddApiVersioning(options => {
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAzureAppConfiguration();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(config =>
{
    config.MapControllers();
});

app.Run();

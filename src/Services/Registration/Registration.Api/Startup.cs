using Microsoft.OpenApi.Models;

namespace CQRSJourney.Registration.Api;

/// <summary>
/// Configures the application's request pipeline.
/// </summary>
public sealed class Startup
{
    /// <summary>
    /// Initializes a new instance of <c>Startup</c> class.
    /// </summary>
    /// <param name="configuration">The application settings.</param>
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// Represents the set application settings.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Configures the application's services by adding them to the container.
    /// Services added here will be available across the application via dependency injection (DI).
    /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    /// </summary>
    /// <param name="services">A collection of services descriptors.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Registration.Api", Version = "v1" });
        });
    }

    /// <summary>
    /// Configures the HTTP request pipeline.
    /// </summary>
    /// <param name="app">A working instance of the ASP.NET runtime to configure the application's request pipeline.</param>
    /// <param name="env">Provides information about the web hosting environment the application is running in.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Registration.Api v1"));

            // Configure the error handler to show an error page.
            // Since IdentityModel version 5.2.1 (or since Microsoft.AspNetCore.Authentication.JwtBearer version 2.2.0),​
            // PII hiding in log files is enabled by default for GDPR concerns.​
            // For debugging/development purposes, one can enable additional detail in exceptions by setting IdentityModelEventSource.ShowPII to true.
            app.UseDeveloperExceptionPage();
            // app.UseMigrationsEndPoint();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        // Add endpoint to the request pipeline.
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

using GemNote.API.Extensions;
using GemNote.API.Services.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services, builder.Configuration);
        
        var app = builder.Build();
        Configure(app, app.Environment, app.Services);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddServices();
        services.AddRepositories();
        services.AddIdentity(configuration);
        services.AddJwtBearer(configuration);
        services.AddSwagger();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost", policy =>
            {
                policy.WithOrigins("https://localhost:7013")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });

            options.AddPolicy("AllowAzureStaticWebApp", policy =>
            {
                policy.WithOrigins("https://lively-water-0e534d00f.5.azurestaticapps.net")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });

            options.AddPolicy("AllowAzureWebApp", policy =>
            {
                policy.WithOrigins("https://gemnoteapi.azurewebsites.net")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });
    }

    private static void Configure(IApplicationBuilder app, IHostEnvironment env, IServiceProvider serviceProvider)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("AllowLocalhost");
        }
        else if (env.IsProduction())
        {
            app.UseCors("AllowAzureWebApp");
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // Seed data
        SeedData(serviceProvider);
    }

    private static void SeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<ISeeder>();

        seeder.SeedRolesAsync().Wait();
        seeder.SeedUsersAsync().Wait();
        seeder.SeedCategoriesAsync().Wait();
        seeder.SeedNotebookAsync().Wait();
    }
}

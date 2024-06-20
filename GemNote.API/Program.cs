using GemNote.API.Extensions;
using GemNote.API.Services.Contracts;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddIdentity(builder.Configuration);
builder.Services.AddJwtBearer(builder.Configuration);
builder.Services.AddSwagger();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("https://localhost:7013") // Local development URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    options.AddPolicy("AllowAzureStaticWebApp", policy =>
    {
        policy.WithOrigins("https://lively-water-0e534d00f.5.azurestaticapps.net") // Production URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
      options.AddPolicy("AllowAzureStaticWebApp", policy =>
    {
        policy.WithOrigins("https://lively-water-0e534d00f.5.azurestaticapps.net/api/auth/register") // Production URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

});

var app = builder.Build();

#region Seeding data

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<ISeeder>();

await seeder.SeedRolesAsync();
await seeder.SeedUsersAsync();
await seeder.SeedCategoriesAsync();
await seeder.SeedNotebookAsync();

#endregion

// Configure the HTTP request pipeline.
app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    app.UseCors("AllowLocalhost");
}
else if (app.Environment.IsProduction())
{
    app.UseCors("AllowAzureStaticWebApp");
}

app.UseHttpsRedirection();
app.UseCors(); // Ensure UseCors is called before UseAuthentication and UseAuthorization
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

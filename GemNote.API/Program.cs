using GemNote.API.Extensions;
using GemNote.API.Services.Contracts;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddServices();

// Add repositories to the container.
builder.Services.AddRepositories();

// Add DbContext
builder.Services.AddIdentity(builder.Configuration);

// Add Authentication and JWT Bearer
builder.Services.AddJwtBearer(builder.Configuration);

builder.Services.AddSwagger();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("*", policy =>
    {
        policy.WithOrigins("https://localhost:7013") // Replace with your local development URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("*", policy =>
    {
        policy.WithOrigins("https://lively-water-0e534d00f.5.azurestaticapps.net") // Replace with your production URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddCorsConfig();

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
	app.UseCors("*");
}
else if (app.Environment.IsProduction())
{
	app.UseCors("*");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

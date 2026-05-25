using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry().UseAzureMonitor();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("StrictSecurityPolicy", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://min-sakra-frontend-app.azurewebsites.net").WithMethods("GET", "POST").AllowAnyHeader();
    });
});

// Azure Key Vault
var keyVaultUrl = builder.Configuration["KeyVaultUrl"];

if (!string.IsNullOrEmpty(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault( new Uri(keyVaultUrl), new DefaultAzureCredential());
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.UseHttpsRedirection();

app.UseCors("StrictSecurityPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
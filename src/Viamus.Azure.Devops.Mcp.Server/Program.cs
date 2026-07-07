using Viamus.Azure.Devops.Mcp.Server.Configuration;
using Viamus.Azure.Devops.Mcp.Server.Middleware;
using Viamus.Azure.Devops.Mcp.Server.Services;
using Viamus.Azure.Devops.Mcp.Server.Tools;

var builder = WebApplication.CreateBuilder(args);

// Configure Azure DevOps options
builder.Services.Configure<AzureDevOpsOptions>(
    builder.Configuration.GetSection(AzureDevOpsOptions.SectionName));

// Configure server security options
builder.Services.Configure<ServerSecurityOptions>(
    builder.Configuration.GetSection(ServerSecurityOptions.SectionName));

// Validate configuration on startup
var azureDevOpsConfig = builder.Configuration.GetSection(AzureDevOpsOptions.SectionName).Get<AzureDevOpsOptions>();
var azureDevOpsValidationErrors = azureDevOpsConfig?.Validate() ?? ["AzureDevOps configuration is required."];
if (azureDevOpsValidationErrors.Count > 0)
{
    throw new InvalidOperationException(
        "AzureDevOps configuration is invalid: " + string.Join(" ", azureDevOpsValidationErrors));
}

// Register services
builder.Services.AddSingleton<IAzureDevOpsOrganizationContextAccessor, AzureDevOpsOrganizationContextAccessor>();
builder.Services.AddSingleton<IAzureDevOpsService, AzureDevOpsService>();

// Configure MCP Server
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// Use API key authentication middleware
app.UseApiKeyAuthentication();

// Map MCP endpoints
app.MapMcp();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();

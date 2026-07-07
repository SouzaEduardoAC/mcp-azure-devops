using Viamus.Azure.Devops.Mcp.Server.Configuration;

namespace Viamus.Azure.Devops.Mcp.Server.Tests.Configuration;

public sealed class AzureDevOpsOptionsTests
{
    [Fact]
    public void GetConfiguredOrganizations_WithLegacySettings_ShouldReturnDefaultOrganization()
    {
        var options = new AzureDevOpsOptions
        {
            OrganizationUrl = "https://dev.azure.com/contoso",
            PersonalAccessToken = "pat",
            DefaultProject = "MainProject"
        };

        var organizations = options.GetConfiguredOrganizations();

        Assert.Single(organizations);
        Assert.Equal("contoso", organizations[0].Name);
        Assert.Equal("https://dev.azure.com/contoso", organizations[0].OrganizationUrl);
        Assert.Equal("pat", organizations[0].PersonalAccessToken);
        Assert.Equal("MainProject", organizations[0].DefaultProject);
    }

    [Fact]
    public void Validate_WithOrganizationsOnly_ShouldSucceed()
    {
        var options = new AzureDevOpsOptions
        {
            DefaultOrganization = "secondary",
            Organizations =
            [
                new AzureDevOpsOrganizationOptions
                {
                    Name = "primary",
                    OrganizationUrl = "https://dev.azure.com/primary",
                    PersonalAccessToken = "primary-pat",
                    DefaultProject = "PrimaryProject"
                },
                new AzureDevOpsOrganizationOptions
                {
                    Name = "secondary",
                    OrganizationUrl = "https://dev.azure.com/secondary",
                    PersonalAccessToken = "secondary-pat",
                    DefaultProject = "SecondaryProject"
                }
            ]
        };

        var errors = options.Validate();

        Assert.Empty(errors);
        Assert.Equal(2, options.GetConfiguredOrganizations().Count);
    }

    [Fact]
    public void Validate_WithMissingOrganizationPat_ShouldReturnError()
    {
        var options = new AzureDevOpsOptions
        {
            Organizations =
            [
                new AzureDevOpsOrganizationOptions
                {
                    Name = "primary",
                    OrganizationUrl = "https://dev.azure.com/primary"
                }
            ]
        };

        var errors = options.Validate();

        Assert.Contains(errors, error => error.Contains("requires PersonalAccessToken", StringComparison.Ordinal));
    }
}

# Azure Pipeline Examples

This repository contains a minimal .NET sample application and four Azure Pipelines definitions that gradually increase in complexity—from a basic continuous integration build to a multi-stage pipeline that provisions infrastructure with Terraform and performs blue-green deployments.

## Repository Layout

```
.
├── AzurePipelineSamples.sln
├── infra/
│   └── terraform/
│       ├── main.tf
│       ├── outputs.tf
│       ├── variables.tf
│       └── versions.tf
├── pipelines/
│   ├── advanced/azure-pipelines.yml
│   ├── basic/azure-pipelines.yml
│   ├── hard/azure-pipelines.yml
│   └── medium/azure-pipelines.yml
├── src/
│   ├── Contoso.App/            # Console host that surfaces the library functionality
│   └── Contoso.Utilities/      # Class library with a Fibonacci generator
└── tests/
    └── Contoso.Utilities.Tests/  # Console-based smoke tests that exercise the library
```

## Local Development

1. **Restore & build**
   ```bash
   dotnet restore AzurePipelineSamples.sln
   dotnet build AzurePipelineSamples.sln --configuration Release
   ```
2. **Run the console app**
   ```bash
   dotnet run --project src/Contoso.App/Contoso.App.csproj -- 12
   ```
   The optional argument controls how many Fibonacci numbers are printed (default: 10).
3. **Execute smoke tests**
   ```bash
   dotnet run --project tests/Contoso.Utilities.Tests/Contoso.Utilities.Tests.csproj
   ```
   The test harness returns a non-zero exit code if any assertion fails, making it suitable for use in CI.

## Pipeline Profiles

| Pipeline | Location | Highlights |
|----------|----------|------------|
| Basic | `pipelines/basic/azure-pipelines.yml` | Restores and builds the solution. Ideal for CI-only scenarios. |
| Medium | `pipelines/medium/azure-pipelines.yml` | Builds, publishes, archives the console app, and deploys to Azure App Service. |
| Advanced | `pipelines/advanced/azure-pipelines.yml` | Multi-stage CI/CD with artifact sharing, console-based tests, and production deployment gated by environment approvals. |
| Hard | `pipelines/hard/azure-pipelines.yml` | Adds Terraform infrastructure provisioning plus blue-green deployment via slot swap. |

Every pipeline targets `.NET SDK 9.x` and expects the solution file to live at the repo root.

### Required Azure Configuration

Before running the Medium, Advanced, or Hard pipelines, create the following in Azure DevOps:

- **Service connection** referencing the subscription that owns your Azure App Service and (optionally) the infrastructure resources.
- **Variable group or pipeline variables** for settings such as `appServiceName`, `resourceGroupName`, and `stagingSlot` (see the defaults inside each YAML file).
- **Environment** named `Production` (and `Production/staging` for the Hard pipeline) if you want to use manual approvals before deployment.

Update the placeholder values (`<Your ...>`) in the YAML files to match your Azure resources.

## Terraform Blueprint (Hard Pipeline)

The Terraform configuration under `infra/terraform` provisions:

- A resource group
- An Azure Linux App Service plan
- A Linux Web App
- A staging deployment slot

It outputs the default hostnames for both production and staging. Supply the variables (e.g., via a `.tfvars` file or pipeline variables) to control names, region, and SKU. The Hard pipeline runs `terraform init` and `terraform apply` with `-auto-approve` before building and deploying the application.

## Extending the Sample

- Add richer domain logic to `Contoso.Utilities` and expand the smoke tests accordingly.
- Replace the console smoke tests with a formal testing framework (xUnit, NUnit, etc.) when a NuGet feed is available.
- Parameterize the pipelines further by turning common steps into templates, or split build/test jobs for larger repositories.

Happy shipping! 🚀

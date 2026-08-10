# Azure Infrastructure

This Terraform configuration deploys the browser game to Azure Linux App Service. It creates:

- One resource group per environment
- A Linux App Service plan and web app with WebSockets enabled for Blazor Server
- A system-assigned managed identity for the web app
- Log Analytics and workspace-based Application Insights

## Prerequisites

- Terraform 1.8 or later
- Azure CLI authenticated with `az login`
- An Azure subscription in which you can create resource groups and App Service resources

## Deploy Infrastructure

```sh
cd infrastructure
cp terraform.tfvars.example terraform.tfvars
# Set subscription_id and adjust the example values.
terraform init
terraform plan -out main.tfplan
terraform apply main.tfplan
```

The configuration uses local Terraform state by default. For shared or production environments, configure an `azurerm` backend and supply its storage account settings during `terraform init`.

## Publish the Web App

From the repository root:

```sh
dotnet publish threenoughtfour.web/ThreeZeroFour.Web.csproj \
  --configuration Release \
  --output .publish

cd .publish
zip -r ../three-zero-four.zip .
cd ..

az webapp deploy \
  --resource-group "$(terraform -chdir=infrastructure output -raw resource_group_name)" \
  --name "$(terraform -chdir=infrastructure output -raw web_app_name)" \
  --src-path three-zero-four.zip \
  --type zip
```

The application URL is available from `terraform -chdir=infrastructure output -raw web_app_url`.

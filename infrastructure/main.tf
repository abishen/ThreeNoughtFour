locals {
  resource_prefix = "${var.workload_name}-${var.environment}"
  common_tags = merge(var.tags, {
    environment = var.environment
    managed-by  = "terraform"
    workload    = var.workload_name
  })
}

resource "random_string" "resource_suffix" {
  length  = 6
  special = false
  upper   = false
}

resource "azurerm_resource_group" "main" {
  name     = "rg-${local.resource_prefix}"
  location = var.location
  tags     = local.common_tags
}

module "monitoring" {
  source = "./modules/monitoring"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  resource_prefix     = local.resource_prefix
  retention_in_days   = var.log_retention_in_days
  tags                = local.common_tags
}

module "app_service" {
  source = "./modules/app_service"

  resource_group_name                    = azurerm_resource_group.main.name
  location                               = azurerm_resource_group.main.location
  resource_prefix                        = local.resource_prefix
  resource_suffix                        = random_string.resource_suffix.result
  service_plan_sku                       = var.service_plan_sku
  dotnet_version                         = var.dotnet_version
  application_insights_connection_string = module.monitoring.application_insights_connection_string
  tags                                   = local.common_tags
}
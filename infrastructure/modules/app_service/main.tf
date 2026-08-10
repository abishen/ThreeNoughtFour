resource "azurerm_service_plan" "main" {
  name                = "asp-${var.resource_prefix}"
  location            = var.location
  resource_group_name = var.resource_group_name
  os_type             = "Linux"
  sku_name            = var.service_plan_sku
  tags                = var.tags
}

resource "azurerm_linux_web_app" "main" {
  name                = "app-${var.resource_prefix}-${var.resource_suffix}"
  location            = var.location
  resource_group_name = var.resource_group_name
  service_plan_id     = azurerm_service_plan.main.id
  https_only          = true
  tags                = var.tags

  identity {
    type = "SystemAssigned"
  }

  site_config {
    always_on           = true
    http2_enabled       = true
    minimum_tls_version = "1.2"
    websockets_enabled  = true

    application_stack {
      dotnet_version = var.dotnet_version
    }
  }

  app_settings = {
    APPLICATIONINSIGHTS_CONNECTION_STRING      = var.application_insights_connection_string
    ApplicationInsightsAgent_EXTENSION_VERSION = "~3"
    ASPNETCORE_ENVIRONMENT                     = "Production"
    WEBSITE_RUN_FROM_PACKAGE                   = "1"
  }
}
output "resource_group_name" {
  description = "Name of the resource group containing the application."
  value       = azurerm_resource_group.main.name
}

output "web_app_name" {
  description = "Globally unique Azure Web App name."
  value       = module.app_service.web_app_name
}

output "web_app_url" {
  description = "HTTPS URL of the deployed game."
  value       = module.app_service.web_app_url
}

output "application_insights_name" {
  description = "Application Insights resource name."
  value       = module.monitoring.application_insights_name
}
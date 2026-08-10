output "application_insights_name" {
  description = "Application Insights resource name."
  value       = azurerm_application_insights.main.name
}

output "application_insights_connection_string" {
  description = "Application Insights connection string for application telemetry."
  value       = azurerm_application_insights.main.connection_string
  sensitive   = true
}
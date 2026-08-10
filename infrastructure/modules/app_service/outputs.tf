output "web_app_name" {
  description = "Azure Web App name."
  value       = azurerm_linux_web_app.main.name
}

output "web_app_url" {
  description = "HTTPS URL of the Azure Web App."
  value       = "https://${azurerm_linux_web_app.main.default_hostname}"
}

output "principal_id" {
  description = "Principal ID of the web app's system-assigned managed identity."
  value       = azurerm_linux_web_app.main.identity[0].principal_id
}
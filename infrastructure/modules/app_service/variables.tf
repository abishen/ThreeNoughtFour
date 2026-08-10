variable "resource_group_name" {
  description = "Resource group in which App Service resources are created."
  type        = string
}

variable "location" {
  description = "Azure region for App Service resources."
  type        = string
}

variable "resource_prefix" {
  description = "Common resource-name prefix."
  type        = string
}

variable "resource_suffix" {
  description = "Random suffix that makes the web app name globally unique."
  type        = string
}

variable "service_plan_sku" {
  description = "Linux App Service plan SKU."
  type        = string
}

variable "dotnet_version" {
  description = "Major .NET runtime version used by the Linux web app."
  type        = string
}

variable "application_insights_connection_string" {
  description = "Application Insights connection string exposed to the web app."
  type        = string
  sensitive   = true
}

variable "tags" {
  description = "Tags applied to App Service resources."
  type        = map(string)
}
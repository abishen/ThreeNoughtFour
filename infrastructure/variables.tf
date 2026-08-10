variable "subscription_id" {
  description = "Azure subscription ID used by the AzureRM provider."
  type        = string
  nullable    = false
}

variable "location" {
  description = "Azure region in which resources are created."
  type        = string
  default     = "eastus2"
}

variable "workload_name" {
  description = "Short lowercase name used in Azure resource names."
  type        = string
  default     = "threezerofour"

  validation {
    condition     = can(regex("^[a-z0-9][a-z0-9-]{1,30}[a-z0-9]$", var.workload_name))
    error_message = "workload_name must be 3-32 lowercase letters, numbers, or hyphens and cannot start or end with a hyphen."
  }
}

variable "environment" {
  description = "Deployment environment name."
  type        = string
  default     = "dev"

  validation {
    condition     = contains(["dev", "test", "staging", "prod"], var.environment)
    error_message = "environment must be dev, test, staging, or prod."
  }
}

variable "service_plan_sku" {
  description = "Linux App Service plan SKU."
  type        = string
  default     = "B1"
}

variable "dotnet_version" {
  description = "Major .NET runtime version used by the Linux web app."
  type        = string
  default     = "10.0"
}

variable "log_retention_in_days" {
  description = "Log Analytics retention period."
  type        = number
  default     = 30

  validation {
    condition     = var.log_retention_in_days >= 30 && var.log_retention_in_days <= 730
    error_message = "log_retention_in_days must be between 30 and 730."
  }
}

variable "tags" {
  description = "Additional tags applied to every resource."
  type        = map(string)
  default     = {}
}
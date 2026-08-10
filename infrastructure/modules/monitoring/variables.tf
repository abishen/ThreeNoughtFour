variable "resource_group_name" {
  description = "Resource group in which monitoring resources are created."
  type        = string
}

variable "location" {
  description = "Azure region for monitoring resources."
  type        = string
}

variable "resource_prefix" {
  description = "Common resource-name prefix."
  type        = string
}

variable "retention_in_days" {
  description = "Log Analytics retention period."
  type        = number
}

variable "tags" {
  description = "Tags applied to monitoring resources."
  type        = map(string)
}
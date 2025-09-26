variable "resource_group_name" {
  description = "Name of the resource group to create."
  type        = string
}

variable "location" {
  description = "Azure region for all resources."
  type        = string
  default     = "eastus"
}

variable "app_service_plan_name" {
  description = "Name of the App Service plan."
  type        = string
}

variable "app_service_name" {
  description = "Name of the App Service (Linux)."
  type        = string
}

variable "sku_name" {
  description = "SKU for the App Service plan."
  type        = string
  default     = "B1"
}

variable "staging_slot_name" {
  description = "Name of the staging slot."
  type        = string
  default     = "staging"
}

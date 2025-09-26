output "app_service_default_hostname" {
  description = "Default hostname for the production slot."
  value       = azurerm_linux_web_app.app.default_hostname
}

output "staging_slot_default_hostname" {
  description = "Default hostname for the staging slot."
  value       = azurerm_linux_web_app_slot.staging.default_hostname
}

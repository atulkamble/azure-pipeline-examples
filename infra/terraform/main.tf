resource "azurerm_resource_group" "app" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_service_plan" "app" {
  name                = var.app_service_plan_name
  resource_group_name = azurerm_resource_group.app.name
  location            = azurerm_resource_group.app.location
  os_type             = "Linux"
  sku_name            = var.sku_name
}

resource "azurerm_linux_web_app" "app" {
  name                = var.app_service_name
  resource_group_name = azurerm_resource_group.app.name
  location            = azurerm_resource_group.app.location
  service_plan_id     = azurerm_service_plan.app.id

  site_config {
    application_stack {
      dotnet_version = "6.0"
    }

    ftps_state = "Disabled"
  }

  lifecycle {
    ignore_changes = [app_settings]
  }
}

resource "azurerm_linux_web_app_slot" "staging" {
  name           = var.staging_slot_name
  app_service_id = azurerm_linux_web_app.app.id

  site_config {
    application_stack {
      dotnet_version = "6.0"
    }

    ftps_state = "Disabled"
  }
}

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.37.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "=2.19.1"
    }
  }
}

# Configure the Microsoft Azure Providers
provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
    app_configuration {
      purge_soft_delete_on_destroy = true
      recover_soft_deleted         = true
    }
  }
}
provider "azuread" {

}

# variables
variable "base_name" {
  description = "Base name to use for the resources"
  type        = string
}

variable "resource_group" {
  description = "Resource group to use for the resources"
  type = object({
    id     = string
    region = string
    name   = string
  })
}

variable "tags" {
  description = "Tags"
  type        = map(string)
}

# outputs
output "stg" {
  value = {
    storage_account = azurerm_storage_account.stg,
    //container_name  = var.storage_container.name
  }
}

# locals
locals {
  storage_account_name = format("stg%s", var.base_name)
  tags = merge(var.tags, {
  })
}

## resources
resource "azurerm_storage_account" "stg" {
  name                     = local.storage_account_name
  resource_group_name      = var.resource_group.name
  location                 = var.resource_group.region
  account_tier             = "Standard"
  account_replication_type = "LRS"
  tags                     = local.tags
}


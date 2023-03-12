## variables
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

variable "eventhub_sku_tier" {
  description = "SKU size"
  type        = string
  default     = "Standard"
}

variable "tags" {
  description = "Tags"
  type        = map(string)
}

## outputs
output "eh" {
  value = ({
    namespace        = azurerm_eventhub_namespace.eh_ns,
    name             = azurerm_eventhub.eh.name
    id               = azurerm_eventhub.eh.id,
    partition_ids    = azurerm_eventhub.eh.partition_ids,
    listen_auth_rule = azurerm_eventhub_authorization_rule.listen_key,
    manage_auth_rule = azurerm_eventhub_authorization_rule.manage_key,
    sender_auth_rule = azurerm_eventhub_authorization_rule.sender_key
  })
}

## locals
locals {
  eventhub_namespace = format("ehns-%s", var.base_name)
  eventhub_name      = format("eh-%s", var.base_name)
  tags = merge(var.tags, {
  })
}

## resources
resource "azurerm_eventhub_namespace" "eh_ns" {
  name                = local.eventhub_namespace
  location            = var.resource_group.region
  resource_group_name = var.resource_group.name
  sku                 = var.eventhub_sku_tier
  capacity            = 1
  tags                = local.tags
}

resource "azurerm_eventhub" "eh" {
  name                = local.eventhub_name
  namespace_name      = azurerm_eventhub_namespace.eh_ns.name
  resource_group_name = var.resource_group.name
  partition_count     = 2
  message_retention   = 1
}

resource "azurerm_eventhub_authorization_rule" "manage_key" {
  name                = format("manage-%s", local.eventhub_name)
  namespace_name      = azurerm_eventhub_namespace.eh_ns.name
  eventhub_name       = azurerm_eventhub.eh.name
  resource_group_name = var.resource_group.name
  listen              = true
  send                = true
  manage              = true
}

resource "azurerm_eventhub_authorization_rule" "listen_key" {
  name                = format("listen-%s", local.eventhub_name)
  namespace_name      = azurerm_eventhub_namespace.eh_ns.name
  eventhub_name       = azurerm_eventhub.eh.name
  resource_group_name = var.resource_group.name
  listen              = true
  send                = false
  manage              = false
}

resource "azurerm_eventhub_authorization_rule" "sender_key" {
  name                = format("sender-%s", local.eventhub_name)
  namespace_name      = azurerm_eventhub_namespace.eh_ns.name
  eventhub_name       = azurerm_eventhub.eh.name
  resource_group_name = var.resource_group.name
  listen              = false
  send                = true
  manage              = false
}

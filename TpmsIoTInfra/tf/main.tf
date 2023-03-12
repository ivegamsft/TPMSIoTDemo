# Outputs
#Run terraform output deployment_info to view
output "deployment_info" {
  value = ({
    resource_group_name    = module.base_rg.rg.name,
    region                 = module.base_rg.rg.region,
    storage_account        = module.storage.stg.storage_account.name,
    storage_account_key    = module.storage.stg.storage_account.primary_access_key,
    eventhub_name          = module.eventhub.eh.name,
    eventhub_id            = module.eventhub.eh.id,
    eventhub_namespace     = module.eventhub.eh.namespace.name,
    eventhub_partition_ids = module.eventhub.eh.partition_ids,
    eventhub_listen_key    = module.eventhub.eh.listen_auth_rule.primary_connection_string,
    eventhub_manage_key    = module.eventhub.eh.manage_auth_rule.primary_connection_string,
    eventhub_sender_key    = module.eventhub.eh.sender_auth_rule.primary_connection_string
  })
  sensitive = true
}


#locals
locals {
  base_name = var.base_name == "random" ? random_string.base_id.result : var.base_name

  #Merge tags for the dployment
  tags = merge(var.tags, {
    "BaseName"       = local.base_name,
    "LastModified"   = formatdate("DD MMM YYYY hh:mm ZZZ", timestamp())
    "DeploymentType" = "tf"
  })
}

resource "random_string" "base_id" {
  length  = 7
  special = false
  upper   = false
  numeric = true
}

# Base Resource group
module "base_rg" {
  source    = "./modules/rg"
  base_name = local.base_name
  location  = var.location
  tags      = local.tags
}

# Storage
module "storage" {
  source         = "./modules/storage"
  base_name      = local.base_name
  resource_group = module.base_rg.rg
  tags           = local.tags
}

module "eventhub" {
  source            = "./modules/eventhub"
  base_name         = local.base_name
  resource_group    = module.base_rg.rg
  eventhub_sku_tier = "Standard"
  tags              = local.tags
}

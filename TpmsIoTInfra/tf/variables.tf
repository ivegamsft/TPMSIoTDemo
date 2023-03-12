# Can be set as variables set as GitHub secrets at action time

## variables
variable "base_name" {
  description = "Base name to use for the resources"
  type        = string
  default     = "random"
}

variable "location" {
  description = "Base region for the resources"
  type        = string
  default     = "southcentralus"
}

variable "iothub_sku_tier" {
  description = "SKU tier"
  type        = string
  default     = "Standard"
}

variable "eventhub_sku_tier" {
  description = "SKU size"
  type        = string
  default     = "Standard"
}

variable "tags" {
  description = "Tags to apply to the resources"
  type        = map(any)
  default     = {}
}
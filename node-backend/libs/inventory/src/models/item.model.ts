import { ItemConsumableModel } from './item.consumable.model'
import { ItemEquipmentModel } from './item.equipment.model'

export class ItemModel {
  itemId: string
  name: string
  description: string

  equipment: ItemEquipmentModel
  consumable: ItemConsumableModel

  droppable: boolean = true
  tradeable: boolean = true
  unique: boolean = false

  constructor(values: Partial<ItemModel> = {}) {
    Object.assign(this, values)
  }
}

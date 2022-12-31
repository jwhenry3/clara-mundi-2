import { ElementalModel, EquipmentModel, EquipmentSlotsModel, FunctionalModel, StatsModel } from '@app/core'

export class ItemEquipmentModel {
  slots: (keyof EquipmentModel)[] = []
  stats: StatsModel = new StatsModel()
  functional: FunctionalModel = new FunctionalModel()
  element: ElementalModel = {}
  constructor(values: Partial<ItemEquipmentModel> = {}) {
    Object.assign(this, values)
  }
}

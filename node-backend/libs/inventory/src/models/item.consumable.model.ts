import { EffectModel, StatusEffectModel } from '@app/core'

export class ItemConsumableModel {
  // status effect to apply if applicable
  status: StatusEffectModel
  effect: EffectModel

  consumeOnUse: boolean = true

  constructor(values: Partial<ItemConsumableModel> = {}) {
    Object.assign(this, values)
  }
}

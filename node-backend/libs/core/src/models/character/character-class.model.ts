import { EquipmentModel } from '../equipment/equipment.model'
import { StatsModel } from '../statistics/stats.model'

export interface CharacterClassModel extends CharacterClassSearchModel {
  isCurrent: boolean
  exp: number

  equipment: EquipmentModel
  stats: StatsModel
}
export interface CharacterClassIdentification {
  classId: string
}
export interface CharacterClassSearchModel
  extends CharacterClassIdentification {
  level: number
}

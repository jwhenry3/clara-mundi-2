import { CharacterClassModel, CharacterClassSearchModel } from './character-class.model'

export interface CharacterModel
  extends CharacterIdentification,
    CharacterLocation,
    CharacterAppearance {
  characterClass: CharacterClassModel
}

export interface CharacterLocation {
  area: string
  position_x: number
  position_y: number
  position_z: number
}

export interface CharacterAppearance {
  race: string
  gender: string
}
export interface CharacterIdentification {
  name: string
}

export interface CharacterSearchModel
  extends CharacterIdentification,
    CharacterLocation,
    CharacterAppearance,
    CharacterClassSearchModel {}

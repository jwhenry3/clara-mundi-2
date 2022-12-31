import { SkillType } from './skill-type.enum'

export type ISKillsModel = {
  [key in SkillType]?: number
}

import {
  Column,
  Entity,
  OneToMany,
  PrimaryColumn,
  PrimaryGeneratedColumn,
  Unique,
} from 'typeorm'

import { CharacterClassEntity } from './character-class.entity'
import { CharacterItemInstanceEntity } from './character-item-instance.entity'

@Unique('name', ['name'])
@Entity('character')
export class CharacterEntity {
  @Column('varchar')
  accountId: string
  @PrimaryGeneratedColumn('uuid')
  id: string
  @Column('varchar')
  name: string
  @Column('varchar')
  gender: string = 'male'
  @Column('varchar')
  race: string = 'human'
  @Column('varchar')
  area: string = 'Rein'
  @Column('decimal', { default: 0 })
  position_x: number = 0
  @Column('decimal', { default: 0 })
  position_y: number = 0
  @Column('decimal', { default: 0 })
  position_z: number = 0
  @Column('decimal', { default: 0 })
  rotation: number = 0

  @Column('int', { default: 0 })
  lastConnected: number = 0
  @Column('int', { default: 0 })
  lastDisconnected: number = 0

  @Column('int', { default: 0 })
  hasConnectedBefore: number = 0

  @OneToMany(() => CharacterClassEntity, (c) => c.character, {
    cascade: true,
    eager: true,
  })
  characterClasses: CharacterClassEntity[]
  @OneToMany(() => CharacterItemInstanceEntity, (c) => c.character, {
    cascade: true,
    eager: true,
  })
  characterItems: CharacterItemInstanceEntity[]
}

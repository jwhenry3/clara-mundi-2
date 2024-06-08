import {
  Column,
  Entity,
  JoinColumn,
  ManyToOne,
  OneToOne,
  PrimaryGeneratedColumn,
} from 'typeorm'

import { CharacterClassEntity } from './character-class.entity'

@Entity('character_class_equipment')
export class CharacterEquipmentEntity {
  @PrimaryGeneratedColumn('uuid')
  entityId: string
  @OneToOne(() => CharacterClassEntity, (c) => c.equipment)
  @JoinColumn()
  characterClass: CharacterClassEntity

  @Column('integer')
  Main: number = -1
  @Column('integer')
  Sub: number = -1
  @Column('integer')
  Ranged: number = -1
  @Column('integer')
  Ammo: number = -1
  @Column('integer')
  Head: number = -1
  @Column('integer')
  Neck: number = -1
  @Column('integer')
  Body: number = -1
  @Column('integer')
  Hands: number = -1
  @Column('integer')
  Back: number = -1
  @Column('integer')
  Waist: number = -1
  @Column('integer')
  Legs: number = -1
  @Column('integer')
  Feet: number = -1
  @Column('integer')
  Ear1: number = -1
  @Column('integer')
  Ear2: number = -1
  @Column('integer')
  Ring1: number = -1
  @Column('integer')
  Ring2: number = -1
}

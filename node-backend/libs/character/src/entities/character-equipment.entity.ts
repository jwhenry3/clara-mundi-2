import { Column, Entity, JoinColumn, OneToOne, PrimaryColumn, PrimaryGeneratedColumn, Relation } from 'typeorm'

import { CharacterClassEntity } from './character-class.entity'

@Entity('character_equipment')
export class CharacterEquipmentEntity {
  @PrimaryGeneratedColumn('uuid')
  equipmentId: string

  @OneToOne(() => CharacterClassEntity, (c) => c.equipment)
  @JoinColumn()
  characterClass: Relation<CharacterClassEntity>
  @Column('varchar', { nullable: true })
  mainHand1: string
  @Column('varchar', { nullable: true })
  offHand1: string
  @Column('varchar', { nullable: true })
  mainHand2: string
  @Column('varchar', { nullable: true })
  offHand2: string

  @Column('varchar', { nullable: true })
  head: string
  @Column('varchar', { nullable: true })
  body: string
  @Column('varchar', { nullable: true })
  hands: string
  @Column('varchar', { nullable: true })
  legs: string
  @Column('varchar', { nullable: true })
  feet: string

  @Column('varchar', { nullable: true })
  neck: string
  @Column('varchar', { nullable: true })
  back: string
}

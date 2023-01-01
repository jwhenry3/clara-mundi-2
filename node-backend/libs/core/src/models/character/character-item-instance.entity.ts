import { Column, Entity, JoinColumn, ManyToOne, PrimaryGeneratedColumn, Relation } from 'typeorm'

import { CharacterEntity } from './character.entity'

@Entity('character_item_instance')
export class CharacterItemInstanceEntity {
  @ManyToOne(() => CharacterEntity, (c) => c.characterItems)
  @JoinColumn()
  character: Relation<CharacterEntity>

  @PrimaryGeneratedColumn('uuid')
  instanceId: string
  @Column('varchar')
  storageId: string
  @Column('varchar')
  itemId: string
  @Column('int')
  quantity: number
}

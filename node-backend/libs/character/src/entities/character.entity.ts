import { Column, Entity, PrimaryColumn, PrimaryGeneratedColumn, Unique } from 'typeorm'

@Unique('name', ['name'])
@Entity('character')
export class CharacterEntity {
  @Column('varchar')
  accountId: string
  @PrimaryColumn('varchar')
  name: string
  @Column('varchar')
  gender: string = 'male'
  @Column('varchar')
  race: string = 'human'
  @Column('varchar')
  area: string = 'Rein'
  @Column('decimal')
  position_x: number = 0
  @Column('decimal')
  position_y: number = 0
  @Column('decimal')
  position_z: number = 0
  @Column('decimal')
  rotation: number = 0

  @Column('int')
  level: number = 1
  @Column('bigint')
  exp: number = 0

  @Column('bigint')
  lastConnected: number = 0
  @Column('bigint')
  lastDisconnected: number = 0

  @Column('tinyint')
  hasConnectedBefore: boolean = false
}
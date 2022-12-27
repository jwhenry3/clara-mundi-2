import { Column, Entity, PrimaryColumn, PrimaryGeneratedColumn, Unique } from 'typeorm'

@Unique('email', ['email'])
@Entity('account')
export class AccountEntity {
  @PrimaryGeneratedColumn('uuid')
  accountId: string
  @Column('varchar')
  email: string
  @Column('varchar')
  password: string
  @Column('int')
  lastLogin: number
  @Column('varchar')
  token: string
}

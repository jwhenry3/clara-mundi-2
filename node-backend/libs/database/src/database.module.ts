import { Module } from '@nestjs/common'
import { ConfigModule } from '@nestjs/config'
import { TypeOrmModule } from '@nestjs/typeorm'

import { DatabaseService } from './database.service'

@Module({
  imports: [
    ConfigModule.forRoot(),
    TypeOrmModule
  ],
  providers: [DatabaseService],
  exports: [DatabaseService, TypeOrmModule, ConfigModule],
})
export class DatabaseModule {

  static forRoot(env: Record<string,string>) {
    return {
      module: DatabaseModule,
      imports: [
        TypeOrmModule.forRoot({
          type: 'postgres',
          host: env.TYPEORM_HOST,
          port: parseInt(env.TYPEORM_PORT || ''),
          username: env.TYPEORM_USERNAME,
          password: env.TYPEORM_PASSWORD,
          database: env.TYPEORM_DATABASE,
          synchronize: true,
          autoLoadEntities: true,
        })
      ]
    }
  }
}

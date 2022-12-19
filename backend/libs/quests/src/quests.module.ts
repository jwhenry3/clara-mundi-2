import { AuthModule } from '@app/auth'
import { CharacterModule } from '@app/character'
import { Module } from '@nestjs/common'

import { HasQuestCompletionsGuard } from './has-quest-completions/has-quest-completions.guard'
import { QuestController } from './quest/quest.controller'
import { QuestsService } from './quests.service'

@Module({
  imports: [AuthModule, CharacterModule],
  providers: [QuestsService, HasQuestCompletionsGuard],
  exports: [QuestsService, HasQuestCompletionsGuard],
  controllers: [QuestController],
})
export class QuestsModule {}

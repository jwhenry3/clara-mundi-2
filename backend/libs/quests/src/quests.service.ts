import { Injectable } from '@nestjs/common'

@Injectable()
export class QuestsService {
  public getAcceptedQuests(characterName: string) {}
  public getTrackedQuests(characterName: string) {}

  public trackQuests(characterName: string, questIds: string[]) {}
  public untrackQuests(characterName: string, questIds: string[]) {}

  public acceptQuests(characterName: string, questIds: string[]) {}
  public abandonQuests(characterName: string, questIds: string[]) {}

  public hasQuestCompletions(characterName: string, questIds: string[]) {}
}

using System;
using System.Collections.Generic;
using ClaraMundi;
using Unisave.Entities;
using Unisave.Facades;
using Unisave.Facets;

namespace Backend.App
{
    public class QuestFacet : Facet
    {
        public List<QuestCompletionEntity> GetQuestCompletions(string characterId)
        {
            AccountEntity account = Auth.GetPlayer<AccountEntity>();
            if (account == null) return new List<QuestCompletionEntity>();
            var character = DB.Find<CharacterEntity>(characterId);
            if (character == null) return new List<QuestCompletionEntity>();
            if (character.Account.TargetId != account.EntityId) return new();
            return DB.TakeAll<QuestCompletionEntity>().Filter((entity => entity.Character.TargetId == characterId))
                .Get();
        }

        public List<QuestTaskProgressEntity> GetTaskProgress(string characterId)
        {
            AccountEntity account = Auth.GetPlayer<AccountEntity>();
            if (account == null) return new List<QuestTaskProgressEntity>();
            var character = DB.Find<CharacterEntity>(characterId);
            if (character == null) return new List<QuestTaskProgressEntity>();
            if (character.Account.TargetId != account.EntityId) return new();
            return DB.TakeAll<QuestTaskProgressEntity>().Filter((entity => entity.Character.TargetId == characterId))
                .Get();
        }

        public bool ServerMarkQuestCompleted(string serverToken, string characterId, string questId)
        {
            var character = DB.Find<CharacterEntity>(characterId);
            if (character == null) return false;
            var completion = DB.TakeAll<QuestCompletionEntity>()
                .Filter((entity => entity.Character.TargetId == characterId && entity.QuestId == questId))
                .FirstOrCreate();
            completion.Character = character;
            completion.QuestId = questId;
            completion.Character = new EntityReference<CharacterEntity>(characterId);
            completion.LastCompletedTimestamp = DateTime.UtcNow;
            completion.Save();
            return true;
        }

        public bool ServerRecordTaskProgress(string serverToken, string characterId, QuestTaskProgressModel model)
        {
            var character = DB.Find<CharacterEntity>(characterId);
            if (character == null) return false;
            var progress = DB.TakeAll<QuestTaskProgressEntity>().Filter((entity =>
                entity.Character.TargetId == characterId && entity.QuestId == model.QuestId &&
                model.QuestTaskId == entity.QuestTaskId)).FirstOrCreate();
            progress.QuestId = model.QuestId;
            progress.QuestTaskId = model.QuestTaskId;
            progress.DialogueCompleted = model.DialogueCompleted;
            progress.DispatchCount = model.DispatchCount;
            progress.ItemsTurnedIn = model.ItemsTurnedIn;
            progress.IsComplete = model.IsComplete;
            progress.Character = character;
            progress.Save();
            return true;
        }
    }
}
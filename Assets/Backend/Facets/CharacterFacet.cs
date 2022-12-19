using System.Collections.Generic;
using Unisave.Entities;
using Unisave.Facades;
using Unisave.Facets;
using UnityEngine.TextCore.Text;

namespace Backend.App
{
    public class CharacterFacet : Facet
    {
        public List<CharacterEntity> GetCharacters()
        {
            AccountEntity account = Auth.GetPlayer<AccountEntity>();
            if (account == null) return new();
            return DB.TakeAll<CharacterEntity>().Filter((entity) => entity.Account.TargetId == account.EntityId).Get();
        }

        public CharacterEntity GetAuthorizedCharacter(string serverToken, string accountId, string token, string characterId)
        {
            var account = DB.Find<AccountEntity>(accountId);
            if (account == null) return null;
            if (account.token != token) return null;
            var character = DB.Find<CharacterEntity>(characterId);
            // empty mapping indicates nothing was added
            return character;
        }

        public bool CreateCharacter(string characterName, string gender, string race)
        {
            AccountEntity account = Auth.GetPlayer<AccountEntity>();
            if (account == null) return false;
            var existing = DB.TakeAll<CharacterEntity>().Filter((entity) => entity.Name.ToLower() == characterName.ToLower()).First();
            if (existing != null) return false;
            var character = new CharacterEntity()
            {
                Account = account,
                Name = characterName.ToLower(),
                Gender = gender.ToLower(),
                Race = race.ToLower()
            };
            character.Save();
            return true;
        }

        public bool DeleteCharacter(string characterName)
        {
            AccountEntity account = Auth.GetPlayer<AccountEntity>();
            if (account == null) return false;
            var existing = DB.TakeAll<CharacterEntity>().Filter((entity) => entity.Name.ToLower() == characterName.ToLower()).First();
            if (existing.Account.TargetId != account.EntityId) return false;
            existing.Delete();
            // remove all attached data
            DB.TakeAll<ItemEntity>().Filter((entity => entity.Character.TargetId == existing.EntityId)).Get().ForEach((i) => i.Delete());
            DB.TakeAll<QuestCompletionEntity>().Filter((entity => entity.Character.TargetId == existing.EntityId)).Get().ForEach((i) => i.Delete());
            DB.TakeAll<QuestTaskProgressEntity>().Filter((entity => entity.Character.TargetId == existing.EntityId)).Get().ForEach((i) => i.Delete());
            return true;
        }

    }
}
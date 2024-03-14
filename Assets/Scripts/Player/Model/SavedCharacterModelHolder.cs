using CharactersStats;
using System.Collections.Generic;
using UnityEngine;
using Prefab;

namespace Player
{
    public interface ISavedCharacterModelHolder
    {
        Stats GetSavedStats(CharacterType type, int receivedID=-1);

        void Init();

    }

    public class SavedCharacterModelHolder :  ISavedCharacterModelHolder
    {
        protected List<CharacterModel> _models = new List<CharacterModel>();
        private ModelSaveSystem _modelSaveSystem= ModelSaveSystem.GetInstance();
        public void Init()
        {

        }
        public Stats GetSavedStats(CharacterType type,int receivedID=-1)
        {
            Stats stats;
            CharacterModel model=_modelSaveSystem.Load(type);
            if(model != null)
            {
                stats = new Stats(model.Speed, model.Health, model.Damage, model.LaunchPower);
                return stats;
            }
            else
            {
                Debug.LogWarning("Saved model with such type was not found");
                return null;
            }
        }
    }
}

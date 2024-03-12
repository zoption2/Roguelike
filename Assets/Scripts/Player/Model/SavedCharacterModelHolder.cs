using Player;
using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prefab;

namespace Player
{
    public interface ISavedCharacterModelHolder
    {
        Stats GetSavedStats(PlayerType type, int receivedID=-1);

        void Init();

    }

    public class SavedCharacterModelHolder :  ISavedCharacterModelHolder
    {
        protected List<SavedPlayerModel> _models = new List<SavedPlayerModel>();
        private ModelSaveSystem _modelSaveSystem= ModelSaveSystem.GetInstance();
        public void Init()
        {

        }
        public Stats GetSavedStats(PlayerType type,int receivedID=-1)
        {
            //for (int i = 0; i < _models.Count; i++)
            //{
            //    var model = _models[i];
            //    if (model.ID.Equals(receivedID))
            //    {
            //        Stats stats = new Stats(model.Speed, model.Health, model.Damage);
            //        return stats;
            //    }
            //}
            //return null
            Stats stats;
            SavedPlayerModel model=_modelSaveSystem.Load(type);
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

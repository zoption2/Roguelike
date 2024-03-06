using Player;
using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public interface ISavedCharacterModelHolder
    {
        Stats GetSavedStats(int receivedID);

        void Init();

    }

    public class SavedCharacterModelHolder :  ISavedCharacterModelHolder
    {
        protected List<SavedModel> _models = new List<SavedModel>();
        public void Init()
        {

        }
        public Stats GetSavedStats(int receivedID)
        {
            for (int i = 0; i < _models.Count; i++)
            {
                var model = _models[i];
                if (model.ID.Equals(receivedID))
                {
                    Stats stats = new Stats(model.Speed, model.Health, model.Damage);
                    return stats;
                }
            }
            Debug.LogWarning("Saved model with such id was not found");
            return null;
        }
    }
}

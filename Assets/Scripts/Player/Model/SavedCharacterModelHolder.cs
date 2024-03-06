using Player;
using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu]
    public class SavedCharacterModelHolder : ScriptableObject, ISavedCharacterModelHolder
    {
        [SerializeField]
        protected List<SavedModel> _models;

        public Stats GetSavedStats(int receivedID)
        {
            for (int i = 0; i < _models.Count; i++)
            {
                var model = _models[i];
                if (model.ID.Equals(receivedID))
                {
                    Stats stats = new Stats(model.Speed, model.Health, model.Damage, model.LaunchPower);
                    return stats;
                }
            }
            Debug.LogWarning("Saved model with such id was not found");
            return null;
        }
    }
}

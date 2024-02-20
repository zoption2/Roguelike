using Prefab;
using UnityEngine;

namespace Player
{
    public class ModelSearcher
    {
        private DefaultPlayerModelHolder _defaultModelHolder;
        private SavedCharacterModelHolder _savedModelHolder;
        public Stats GetStats(int id, PlayerType playerType)
        {
            Stats stats;
            stats = _savedModelHolder.GetSavedStats(id);
            if (stats == null)
            {
                stats = _defaultModelHolder.GetDefaultStats(playerType);
            }
            return stats;
        }
    }
}

using Prefab;
using UnityEngine;

namespace Player
{

    public interface IStatsProvider
    {
        public Stats GetStats(PlayerType playerType, int id = -1);
    }
    public class StatsProvider : IStatsProvider
    {
        private DefaultPlayerModelHolder _defaultModelHolder;
        private SavedCharacterModelHolder _savedModelHolder;
        public Stats GetStats(PlayerType playerType, int id = -1)
        {
            Stats stats;
            stats = _savedModelHolder.GetSavedStats(id);
            if (stats == null)
            {
                stats = _defaultModelHolder.GetDefaultStats(playerType);
            }
            return stats;
        }

        public StatsProvider(DefaultPlayerModelHolder defaultModelHolder, SavedCharacterModelHolder savedModelHolder)
        {
            _defaultModelHolder = defaultModelHolder;
            _savedModelHolder = savedModelHolder;
        }
    }
}

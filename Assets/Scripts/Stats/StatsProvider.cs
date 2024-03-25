using Prefab;
using Player;
using Enemy;
using UnityEngine;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace CharactersStats
{
    public interface IStatsProvider
    {
        public OriginStats GetPlayerStats(CharacterType playerType);
        public OriginStats GetEnemyStats(CharacterType enemyType);
    }

    public class StatsProvider : IStatsProvider
    {
        private DefaultPlayerModelHolder _defaultModelHolder;
        private ISavedCharacterModelHolder _savedModelHolder;

        private DefaultEnemyModelHolder _defaultEnemyModelHolder;

        public OriginStats GetPlayerStats(CharacterType playerType)
        {
            OriginStats stats;
            stats = _savedModelHolder.GetSavedStats(playerType);
            Debug.LogWarning(" Not Found Save: " + (stats == null));
            if (stats == null)
            {
                stats = _defaultModelHolder.GetDefaultStats(playerType);
                Debug.LogWarning("Took default" + stats.Health);
            }

            return stats;
        }

        public OriginStats GetEnemyStats(CharacterType enemyType)
        {
            OriginStats stats;

            stats = _defaultEnemyModelHolder.GetDefaultStats(enemyType);

            return stats;
        }

        public StatsProvider(DefaultPlayerModelHolder defaultModelHolder, ISavedCharacterModelHolder savedModelHolder, DefaultEnemyModelHolder defaultEnemyModelHolder)
        {
            _defaultModelHolder = defaultModelHolder;
            _savedModelHolder = savedModelHolder;
            _defaultEnemyModelHolder = defaultEnemyModelHolder;
        }
    }
}

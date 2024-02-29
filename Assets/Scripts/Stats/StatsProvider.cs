using Prefab;
using Player;
using Enemy;

namespace CharactersStats
{

    public interface IStatsProvider
    {
        public Stats GetPlayerStats(PlayerType playerType, int id = -1);
        public Stats GetEnemyStats(EnemyType enemyType, int id = -1);
    }
    public class StatsProvider : IStatsProvider
    {
        private DefaultPlayerModelHolder _defaultModelHolder;
        private SavedCharacterModelHolder _savedModelHolder;

        private DefaultEnemyModelHolder _defaultEnemyModelHolder;
        public Stats GetPlayerStats(PlayerType playerType, int id = -1)
        {
            Stats stats;
            stats = _savedModelHolder.GetSavedStats(id);
            if (stats == null)
            {
                stats = _defaultModelHolder.GetDefaultStats(playerType);
            }
            return stats;
        }

        public Stats GetEnemyStats(EnemyType enemyType, int id = -1)
        {
            Stats stats;

            stats = _defaultEnemyModelHolder.GetDefaultStats(enemyType);
            
            return stats;
        }

        public StatsProvider(DefaultPlayerModelHolder defaultModelHolder, SavedCharacterModelHolder savedModelHolder, DefaultEnemyModelHolder defaultEnemyModelHolder)
        {
            _defaultModelHolder = defaultModelHolder;
            _savedModelHolder = savedModelHolder;
            _defaultEnemyModelHolder = defaultEnemyModelHolder;
        }
    }
}

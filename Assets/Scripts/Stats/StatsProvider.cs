using Prefab;
using Player;
using Enemy;
using UnityEngine;
using SaveSystem;

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
        private IDataService _dataService;

        private DefaultEnemyModelHolder _defaultEnemyModelHolder;

        public OriginStats GetPlayerStats(CharacterType playerType)
        {
            OriginStats stats;
            stats = _dataService.PlayerStats.GetStats(playerType);
            if (stats == null)
            {
                stats = _defaultModelHolder.GetDefaultStats(playerType);
            }
            return stats;
        }

        public OriginStats GetEnemyStats(CharacterType enemyType)
        {
            OriginStats stats;

            stats = _defaultEnemyModelHolder.GetDefaultStats(enemyType);

            return stats;
        }



        public StatsProvider(DefaultPlayerModelHolder defaultModelHolder, IDataService dataService, DefaultEnemyModelHolder defaultEnemyModelHolder)
        {
            _defaultModelHolder = defaultModelHolder;
            _dataService = dataService;
            _defaultEnemyModelHolder = defaultEnemyModelHolder;
        }
    }
}

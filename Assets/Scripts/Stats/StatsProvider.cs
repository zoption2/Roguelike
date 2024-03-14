using Prefab;
using Player;
using Enemy;
using System;
using UnityEditor;
using UnityEngine;
using SaveSystem;

namespace CharactersStats
{

    public interface IStatsProvider
    {
        public Stats GetPlayerStats(PlayerType playerType);
        public Stats GetEnemyStats(EnemyType enemyType);

    }

    public class StatsProvider : IStatsProvider
    {
        private DefaultPlayerModelHolder _defaultModelHolder;
        private IDataService _dataService;

        private DefaultEnemyModelHolder _defaultEnemyModelHolder;

        public Stats GetPlayerStats(PlayerType playerType)
        {
            Stats stats;
            stats = _dataService.PlayerStats.GetStats(playerType);
            if (stats == null)
            {
                stats = _defaultModelHolder.GetDefaultStats(playerType);
            }
            return stats;
        }

        public Stats GetEnemyStats(EnemyType enemyType)
        {
            Stats stats;

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

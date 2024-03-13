using Prefab;
using Player;
using Enemy;
using System;
using UnityEditor;
using UnityEngine;

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
        private ISavedCharacterModelHolder _savedModelHolder;

        private DefaultEnemyModelHolder _defaultEnemyModelHolder;

        public Stats GetPlayerStats(PlayerType playerType, int id = -1)
        {
            Stats stats;
            stats = _savedModelHolder.GetSavedStats(playerType);
            Debug.LogWarning(" Not Found Save: " + (stats == null));
            if (stats == null)
            {
                stats = _defaultModelHolder.GetDefaultStats(playerType);
                Debug.LogWarning("Took default" + stats.Health);
            }

            return stats;
        }

        public Stats GetEnemyStats(EnemyType enemyType, int id = -1)
        {
            Stats stats;

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

using Prefab;
using Player;
using Enemy;
using UnityEngine;
using SaveSystem;
using System.Collections.Generic;

namespace CharactersStats
{
    public interface IStatsProvider
    {
        public OriginStats GetPlayerStats(CharacterType playerType);
        public OriginStats GetEnemyStats(CharacterType enemyType);
        public List<InteractionType> GetEnemyAbilitiesTypes(CharacterType characterType);
        public List<InteractionType> GetPlayerAbilitiesTypes(CharacterType characterType);
    }

    public class StatsProvider : IStatsProvider
    {
        private DefaultPlayerModelHolder _defaultModelHolder;
        private IDataService _dataService;

        private DefaultEnemyModelHolder _defaultEnemyModelHolder;

        public OriginStats GetPlayerStats(CharacterType playerType)
        {
            OriginStats stats;
            stats = _dataService.PlayerData.GetStats(playerType);
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

        public List<InteractionType> GetEnemyAbilitiesTypes(CharacterType characterType)
        { 
            return _defaultEnemyModelHolder.GetAllAbilities(characterType);
        }

        public List<InteractionType> GetPlayerAbilitiesTypes(CharacterType characterType)
        {
            return _defaultModelHolder.GetAllAbilities(characterType);
        }

        public StatsProvider(DefaultPlayerModelHolder defaultModelHolder, IDataService dataService, DefaultEnemyModelHolder defaultEnemyModelHolder)
        {
            _defaultModelHolder = defaultModelHolder;
            _dataService = dataService;
            _defaultEnemyModelHolder = defaultEnemyModelHolder;
        }
    }
}

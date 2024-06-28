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
        public OriginStats GetDefaultPlayerStats(CharacterType playerType);
        public OriginStats GetEnemyStats(CharacterType enemyType);
        public List<AbilityType> GetCharacterAbilitiesTypes(CharacterType characterType);
    }

    public class StatsProvider : IStatsProvider
    {
        private DefaultCharacterModelHolder _defaultModelHolder;
        private IDataService _dataService;

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

        public OriginStats GetDefaultPlayerStats(CharacterType playerType)
        {
            OriginStats stats = _defaultModelHolder.GetDefaultStats(playerType);
            return stats;
        }

        public OriginStats GetEnemyStats(CharacterType enemyType)
        {
            OriginStats stats;

            stats = _defaultModelHolder.GetDefaultStats(enemyType);

            return stats;
        }

        public List<AbilityType> GetCharacterAbilitiesTypes(CharacterType characterType)
        {
            return _defaultModelHolder.GetAllAbilitiesTypes(characterType);
        }

        public StatsProvider(DefaultCharacterModelHolder defaultModelHolder, IDataService dataService)
        {
            _defaultModelHolder = defaultModelHolder;
            _dataService = dataService;
        }
    }
}

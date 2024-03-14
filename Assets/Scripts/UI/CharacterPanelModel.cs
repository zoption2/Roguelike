using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveSystem;
using CharactersStats;

namespace UI
{
    public interface ICharacterPanelModel
    {
        public PlayerModel Model { get; }
        public PlayerType PlayerCharacterType { get; set; }
        public void Init(PlayerType playerType);
    }
    public class CharacterPanelModel : ICharacterPanelModel
    {
        public PlayerModel Model { get; private set; }
        public PlayerType PlayerCharacterType { get; set; }
        private IDataService _dataService;

        public CharacterPanelModel(IDataService dataService)
        {

            _dataService = dataService;
        }
        public void Init(PlayerType playerType)
        {
            PlayerCharacterType = playerType;
            Stats stats = _dataService.PlayerStats.GetStats(playerType);
            Model = new PlayerModel(stats,playerType);
        }
    }
}

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
        public CharacterModel Model { get; }
        public CharacterType PlayerCharacterType { get; set; }
        public void Init(CharacterType playerType);
    }
    public class CharacterPanelModel : ICharacterPanelModel
    {
        public CharacterModel Model { get; private set; }
        public CharacterType PlayerCharacterType { get; set; }
        private ModelSaveSystem _saveSystem;

        public void Init(CharacterType playerType)
        {
            PlayerCharacterType = playerType;
            Stats stats = _dataService.PlayerStats.GetStats(playerType);
            Model = new PlayerModel(stats,playerType);
        }
    }
}

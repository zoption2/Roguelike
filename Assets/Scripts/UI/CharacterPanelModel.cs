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
        public CharacterType PlayerCharacterType { get; set; }
        public void Init(CharacterType playerType);
    }
    public class CharacterPanelModel : ICharacterPanelModel
    {
        public CharacterType PlayerCharacterType { get; set; }
        private IDataService _dataService;

        public CharacterPanelModel(IDataService dataService)
        {

            _dataService = dataService;
        }
        public void Init(CharacterType playerType)
        {
            PlayerCharacterType = playerType;
            OriginStats stats = _dataService.PlayerStats.GetStats(playerType);
        }
    }
}
using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public interface ICharacterPanelModel
    {
        public SavedPlayerModel Model { get; }
        public PlayerType PlayerCharacterType { get; set; }
        public void Init(PlayerType playerType);
    }
    public class CharacterPanelModel : ICharacterPanelModel
    {
        public SavedPlayerModel Model { get; private set; }
        public PlayerType PlayerCharacterType { get; set; }
        private ModelSaveSystem _saveSystem;

        public void Init(PlayerType playerType)
        {
            PlayerCharacterType = playerType;
            _saveSystem = ModelSaveSystem.GetInstance();
            Model = _saveSystem.Load(PlayerCharacterType);
        }
    }
}

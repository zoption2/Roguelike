using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public interface ICharacterPanelModel
    {
        public PlayerModel IDModel { get; }
        public PlayerType PlayerCharacterType { get; set; }
        public void Init(PlayerType playerType);
    }
    public class CharacterPanelModel : ICharacterPanelModel
    {
        public PlayerModel IDModel { get; private set; }
        public PlayerType PlayerCharacterType { get; set; }
        private ModelSaveSystem _saveSystem;

        public void Init(PlayerType playerType)
        {
            PlayerCharacterType = playerType;
            _saveSystem = ModelSaveSystem.GetInstance();
            IDModel = _saveSystem.Load(PlayerCharacterType);
        }
    }
}

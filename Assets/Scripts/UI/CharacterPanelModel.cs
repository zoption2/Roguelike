using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            _saveSystem = ModelSaveSystem.GetInstance();
            Model = _saveSystem.Load(PlayerCharacterType);
        }
    }
}

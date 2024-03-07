using CharactersStats;
using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.IO;
using UnityEditor;

namespace UI
{
    public interface ICharacterPanel
    {
        public SavedModel IDModel { get;}
    }
    public class CharacterPanelView : MonoBehaviour, ICharacterPanel
    {
        
        public SavedModel IDModel { get; private set; }
        [field: SerializeField] public PlayerType PlayerType { get; set; }
        private Toggle _toggle;
       
        private bool _isEnabled;
        private ModelSaveSystem _saveSystem;


        void Start()
        {
            _saveSystem = ModelSaveSystem.GetInstance();
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(ChangeBool);
            Init();
        }

        public void Init()
        {
            IDModel = _saveSystem.Load(PlayerType);
        }
        
        public void ChangeBool(bool toggleValue)
        {
            _isEnabled = toggleValue;
            if(_isEnabled)
            {
                //DataTransfer.IdCollection.Add(savedModel.ID);
            }
        }

        
    }
}

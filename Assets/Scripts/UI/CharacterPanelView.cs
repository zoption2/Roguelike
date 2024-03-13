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
using Pool;

namespace UI
{
    public interface ICharacterPanelView
    {
        public void RevertInteractibility();
        public void Init(IPanelClickChange panelValue);
        public PlayerType PlayerType { get; set; }
    }
    
    public class CharacterPanelView : MonoBehaviour, ICharacterPanelView, IMyPoolable
    {
        
        public PlayerModel IDModel { get; private set; }
        [field: SerializeField] public PlayerType PlayerType { get; set; }
        private Toggle _toggle;
        private IPanelClickChange _valueChange;

        public void Init(IPanelClickChange panelValue)
        {
            _toggle = GetComponent<Toggle>();
            _valueChange = panelValue;
            _toggle.onValueChanged.AddListener(_valueChange.ChangeBool);
        }

        public void RevertInteractibility()
        {
            _toggle.interactable = !_toggle.interactable;
        }
        public void OnCreate()
        {
            
        }

        public void OnPull()
        {
            
        }

        public void OnRelease()
        {
            
        }
    }
}

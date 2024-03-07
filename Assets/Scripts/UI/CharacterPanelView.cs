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
    public interface ICharacterPanelView
    {
        public void Init(IPanelClickChange panelValue);
        public PlayerType PlayerType { get; set; }
    }
    
    public class CharacterPanelView : MonoBehaviour, ICharacterPanelView
    {
        
        public SavedModel IDModel { get; private set; }
        [field: SerializeField] public PlayerType PlayerType { get; set; }
        private Toggle _toggle;
        private IPanelClickChange _valueChange;

        public void Init(IPanelClickChange panelValue)
        {
            _toggle = GetComponent<Toggle>();
            _valueChange = panelValue;
            _toggle.onValueChanged.AddListener(_valueChange.ChangeBool);
        }
    }
}

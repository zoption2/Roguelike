using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;


namespace UI
{
    public interface IPanelClickChange
    {
        void ChangeBool(bool toggleValue);

    }

    public interface ICharacterPanelController
    {
        public void Init(ICharacterPanelView panelView, ICharacterPanelModel panelModel);
        public CharacterType GetModelType();
        public void RevertInteract();
        public bool _isEnabled { get; set; }
    }
    public class CharacterPanelController : IPanelClickChange, ICharacterPanelController
    {
        public bool _isEnabled { get; set; }
        private ICharacterPanelModel _panelModel;
        private ICharacterPanelView _panelView;
        private ICharacterSelector _characterSelector;
        public CharacterPanelController(ICharacterSelector characterSelector)
        {
            _characterSelector = characterSelector;
        }
        public void ChangeBool(bool toggleValue)
        {
            _isEnabled = toggleValue;
            if (_isEnabled)
            {
                _characterSelector.SelectPanel(this);
            }
            else
            {
                _characterSelector.UnSelectPanel(this);
            }
        }

        public void RevertInteract()
        {
            _panelView.RevertInteractibility();
        }
        public CharacterType GetModelType()
        {
            return _panelModel.PlayerCharacterType;
        }

        public void Init(ICharacterPanelView panelView, ICharacterPanelModel panelModel)
        {
            _panelModel = panelModel;
            _panelView = panelView;
            _panelView.Init(this);
            _panelModel.Init(_panelView.CharacterType);
        }
    }
}

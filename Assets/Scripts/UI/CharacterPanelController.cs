using Player;
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

        public int GetModelID()
        {
            SavedModel model = _panelModel.IDModel;
            return model.ID;
        }

        public void Init(ICharacterPanelView panelView, ICharacterPanelModel panelModel)
        {
            _panelModel = panelModel;
            _panelView = panelView;
            _panelView.Init(this);
            _panelModel.Init(_panelView.PlayerType);
        }
    }
}

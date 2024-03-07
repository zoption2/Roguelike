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
    public class CharacterPanelController : IPanelClickChange
    {
        private bool _isEnabled;
        private ICharacterPanelModel _panelModel;
        private ICharacterPanelView _panelView;
        public void ChangeBool(bool toggleValue)
        {
            _isEnabled = toggleValue;
            if (_isEnabled)
            {
                //DataTransfer.IdCollection.Add(savedModel.ID);
            }
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

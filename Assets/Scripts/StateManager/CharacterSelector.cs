using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public interface ICharacterSelector
    {
        void AddPanel(PlayerType playerType);
        public RectTransform RectTrans { get; set; }
        public void Init(int requiredPlayersNumber, RectTransform rectTransform);
        public void SelectPanel(ICharacterPanelController controller);
        public void UnSelectPanel(ICharacterPanelController controller);
    }
    public class CharacterSelector : ICharacterSelector
    {
        private List<ICharacterPanelController> _availablePanels = new List<ICharacterPanelController>();
        private List<ICharacterPanelController> _selectedPanels= new List<ICharacterPanelController>();
        private ICharacterPanelFactory _characterPanelFactory;
        private ModelSaveSystem _modelSaveSystem;
        private int _requiredPlayers;
        public RectTransform RectTrans { get; set; }
        public CharacterSelector(ICharacterPanelFactory characterPanelFactory)
        {
            _characterPanelFactory = characterPanelFactory;
        }

        public void SelectPanel(ICharacterPanelController controller)
        {
            //if(_selectedPanels.Count + 1 > _requiredPlayers)
            //{
            //    //UnSelectPanel(_selectedPanels[0]);
            //    _selectedPanels.Add(controller);
            //}
            if (_selectedPanels.Count + 1 <= _requiredPlayers)
            {
                _selectedPanels.Add(controller);
            }
            else
                Debug.LogWarning("You can't select more characters than required!");
        }

        public void UnSelectPanel(ICharacterPanelController controller)
        {
            _selectedPanels.Remove(controller);
        }

        public void Init(int requiredPlayersNumber,RectTransform rectTransform)
        {
            RectTrans = rectTransform;
            _requiredPlayers = requiredPlayersNumber;
            _modelSaveSystem = ModelSaveSystem.GetInstance();
            List<PlayerType> availablePlayers = new List<PlayerType>();
            availablePlayers = _modelSaveSystem.GetAvailablePlayersTypes();
            foreach (PlayerType playerType in availablePlayers)
            {
                AddPanel(playerType);
            }
        }
        public void AddPanel(PlayerType playerType)
        {
            ICharacterPanelController controller = _characterPanelFactory.CreateCharacterPanel(playerType, RectTrans);
            _availablePanels.Add(controller);
        }
    }
}

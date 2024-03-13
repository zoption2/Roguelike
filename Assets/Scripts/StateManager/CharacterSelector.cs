using Gameplay;
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
        public bool HasRequiredNumberOfPlayers();
    }
    public class CharacterSelector : ICharacterSelector
    {
        private List<ICharacterPanelController> _availablePanels = new List<ICharacterPanelController>();
        private List<ICharacterPanelController> _unSelectedPanels = new List<ICharacterPanelController>();
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
            if (_selectedPanels.Count + 1 <= _requiredPlayers)
            {
                _selectedPanels.Add(controller);
                _unSelectedPanels.Remove(controller);
                if (_selectedPanels.Count == _requiredPlayers)
                    MakeUnselectedRevertInteract();
                DataTransfer.TypeCollection.Add(controller.GetModelType());
            }
        }

        public bool HasRequiredNumberOfPlayers()
        {
            return _requiredPlayers == _selectedPanels.Count;
        }
        public void MakeUnselectedRevertInteract()
        {
            foreach(ICharacterPanelController controller in _unSelectedPanels)
            {
                controller.RevertInteract();
            }
        }
        public void UnSelectPanel(ICharacterPanelController controller)
        {
            _selectedPanels.Remove(controller);
            if(_selectedPanels.Count + 1 == _requiredPlayers)
                MakeUnselectedRevertInteract();
            _unSelectedPanels.Add(controller);
            DataTransfer.TypeCollection.Remove(controller.GetModelType());
        }

        public void Init(int requiredPlayersNumber,RectTransform rectTransform)
        {
            DataTransfer.ClearCollections();
            RectTrans = rectTransform;
            _requiredPlayers = requiredPlayersNumber;
            _modelSaveSystem = ModelSaveSystem.GetInstance();
            List<PlayerType> availablePlayers = new List<PlayerType>();
            availablePlayers = _modelSaveSystem.GetAvailablePlayersTypes();
            foreach (PlayerType playerType in availablePlayers)
            {
                AddPanel(playerType);
            }
            _unSelectedPanels = _availablePanels;
        }
        public void AddPanel(PlayerType playerType)
        {
            ICharacterPanelController controller = _characterPanelFactory.CreateCharacterPanel(playerType, RectTrans);
            _availablePanels.Add(controller);
        }
    }
}

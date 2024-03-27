using Gameplay;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveSystem;

namespace UI
{
    public interface ICharacterSelector
    {
        void AddPanel(CharacterType characterType);
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
        private IDataService _dataService;
        private int _requiredPlayers;
        public RectTransform RectTrans { get; set; }
        public CharacterSelector(ICharacterPanelFactory characterPanelFactory, IDataService dataService)
        {
            _characterPanelFactory = characterPanelFactory;
            _dataService = dataService;
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

        public void Init(int requiredPlayersNumber, RectTransform rectTransform)
        {
            DataTransfer.ClearCollections();
            RectTrans = rectTransform;
            _requiredPlayers = requiredPlayersNumber;
            List<CharacterType> availablePlayers = new List<CharacterType>();
            availablePlayers = _dataService.PlayerStats.GetAvailablePlayers();
            if (availablePlayers != null)
            {
                foreach (CharacterType CharacterType in availablePlayers)
                {
                    AddPanel(CharacterType);
                }
                _unSelectedPanels = _availablePanels;
            }

        }
        public void AddPanel(CharacterType characterType)
        {
            ICharacterPanelController controller = _characterPanelFactory.CreateCharacterPanel(characterType, RectTrans);
            _availablePanels.Add(controller);
            _unSelectedPanels.Add(controller);
        }
    }
}

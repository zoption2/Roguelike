using UnityEngine;
using UnityEngine.UI;
using Pool;

namespace UI
{
    public interface ICharacterPanelView
    {
        public void RevertInteractibility();
        public void Init(IPanelClickChange panelValue, ICharacterPanelController characterPanelController);
        public CharacterType CharacterType { get; set; }
        public ICharacterPanelController GetCharacterPanelController();
        public GameObject GameObject { get; }
    }
    
    public class CharacterPanelView : MonoBehaviour, ICharacterPanelView, IMyPoolable
    {
        public CharacterModel Model { get; private set; }
        [field: SerializeField] public CharacterType CharacterType { get; set; }
        private Toggle _toggle;
        private IPanelClickChange _valueChange;
        private ICharacterPanelController _characterPanelController;
        public GameObject GameObject => gameObject;

        public void Init(IPanelClickChange panelValue, ICharacterPanelController characterPanelController)
        {
            _characterPanelController = characterPanelController;
            _toggle = GetComponent<Toggle>();
            _valueChange = panelValue;
            _toggle.onValueChanged.AddListener(_valueChange.ChangeBool);
        }

        public ICharacterPanelController GetCharacterPanelController() { return _characterPanelController; }

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

using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace UI
{
    public interface IUIManager
    {
        void CreateUIElement(UIElementType type);
        void ShowUIElement(UIElementType type);
        void HideUIElement(UIElementType type);
        IUIWindowController GetUIElement(UIElementType type);
    }

    public class UIManager : IUIManager
    {
        private Dictionary<UIElementType, IUIWindowController> _uiElements = new Dictionary<UIElementType, IUIWindowController>();
        private IUIWindowController _activeUIElement;
        private UIPrefabHolder _UIPrefabHolder;
        private GameObject _uiParent;
        private IUIWindowFactory _uiWindowFactory;

        [Inject]
        public UIManager(UIPrefabHolder uIPrefabHolder, IUIWindowFactory uIWindowFactory)
        {
            _UIPrefabHolder = uIPrefabHolder;
            _uiWindowFactory = uIWindowFactory;
            _uiParent = new GameObject("UI");
        }

        public void CreateUIElement(UIElementType type)
        {
            if (!_uiElements.ContainsKey(type))
            {
                var uiElement = _uiWindowFactory.CreateWindow(type, _uiParent.transform);
                _uiElements.Add(type, uiElement);
            }
            else
            {
                Debug.LogWarning($"UI Element of type {type} already exists in the dictionary.");
            }
        }

        public void ShowUIElement(UIElementType type)
        {
            if (_uiElements.ContainsKey(type))
            {
                if (_activeUIElement != null)
                {
                    _activeUIElement.SetDisactive();
                }

                IUIWindowController uiElement = _uiElements[type];
                uiElement.SetActive();
                _activeUIElement = uiElement;
            }
            else
            {
                Debug.LogError($"UI Element of type {type} does not exist in the dictionary.");
            }
        }

        public void HideUIElement(UIElementType type)
        {
            if (_uiElements.ContainsKey(type))
            {
                IUIWindowController uiElement = _uiElements[type];
                uiElement.SetDisactive();

                if (_activeUIElement == uiElement)
                {
                    _activeUIElement = null;
                }
            }
            else
            {
                Debug.LogError($"UI Element of type {type} does not exist in the dictionary.");
            }
        }

        public IUIWindowController GetUIElement(UIElementType type)
        {
            if (_uiElements.ContainsKey(type))
            {
                return _uiElements[type];
            }
            else
            {
                Debug.LogError($"UI Element of type {type} does not exist in the dictionary.");
                return null;
            }
        }
    }
}

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
        GameObject GetUIElement(UIElementType type);
    }

    public class UIManager : IUIManager
    {

        private Dictionary<UIElementType, GameObject> _uiElements = new Dictionary<UIElementType, GameObject>();
        private GameObject _activeUIElement;
        private UIPrefabHolder _UIPrefabHolder;
        private GameObject _uiParent;

        [Inject]
        public UIManager(UIPrefabHolder uIPrefabHolder)
        {
            _UIPrefabHolder = uIPrefabHolder;
            _uiParent = new GameObject("UI");
        }

        public void CreateUIElement(UIElementType type)
        {
            if (!_uiElements.ContainsKey(type))
            {
                GameObject prefab = _UIPrefabHolder.GetPrefab(type);
                GameObject uiElement = GameObject.Instantiate(prefab);
                uiElement.transform.SetParent(_uiParent.transform, false);
                uiElement.SetActive(false);
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
                    _activeUIElement.SetActive(false);
                }

                GameObject uiElement = _uiElements[type];
                uiElement.SetActive(true);
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
                GameObject uiElement = _uiElements[type];
                uiElement.SetActive(false);

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

        public GameObject GetUIElement(UIElementType type)
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






using Pool;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace UI
{
    public interface ICharacterPanelFactory
    {
        ICharacterPanelController CreateCharacterPanel(CharacterType panelType, RectTransform transform);
        // you should also add a reference to the grid in which the panel should be added
    }
    public class CharacterPanelFactory : ICharacterPanelFactory
    {
        private DiContainer _container;
        private CharacterPanelPooler _pooler;
        public CharacterPanelFactory(DiContainer diContainer, CharacterPanelPooler pooler)
        {
            _container = diContainer;
            _pooler = pooler;
            _pooler.Init();
        }
        public ICharacterPanelController CreateCharacterPanel(CharacterType panelType,RectTransform transform)
        {
            ICharacterPanelView panelView;
            ICharacterPanelModel panelModel;
            ICharacterPanelController controller;
            IMyPoolable myPoolable = _pooler.Pull<IMyPoolable>( panelType,new Vector2(0,0),Quaternion.identity,transform);
            panelView = myPoolable.gameObject.GetComponent<CharacterPanelView>();
            panelView.CharacterType = panelType;
            panelModel = _container.Resolve<ICharacterPanelModel>();
            controller = _container.Resolve<ICharacterPanelController>();
            controller.Init(panelView, panelModel);
            return controller;
        }
    }
}

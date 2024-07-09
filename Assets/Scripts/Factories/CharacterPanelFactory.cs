using Pool;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

namespace UI
{
    public interface ICharacterPanelFactory
    {
        ICharacterPanelController CreateCharacterPanel(CharacterType panelType, RectTransform transform);
    }
    public class CharacterPanelFactory : ICharacterPanelFactory
    {
        private DiContainer _container;
        private CharacterPanelPooler _pooler;

        public CharacterPanelFactory(DiContainer diContainer, IPoolManager poolManager)
        {
            _container = diContainer;
            _pooler = poolManager.UseCharacterPanelPooler();
            //_pooler.Init();
        }

        public ICharacterPanelController CreateCharacterPanel(CharacterType panelType,RectTransform transform)
        {
            ICharacterPanelView panelView;
            ICharacterPanelModel panelModel;
            ICharacterPanelController controller;

            IMyPoolable myPoolable =  _pooler.Pull<IMyPoolable>( panelType,new Vector2(0,0),Quaternion.identity,transform);
            panelView = myPoolable.gameObject.GetComponent<CharacterPanelView>();
            panelView.CharacterType = panelType;
            panelModel = _container.Resolve<ICharacterPanelModel>();
            controller = _container.Resolve<ICharacterPanelController>();
            controller.Init(panelView, panelModel);
            return controller;
        }
    }
}

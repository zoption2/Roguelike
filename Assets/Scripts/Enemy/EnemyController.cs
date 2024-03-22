using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using BehaviourTree;
using CharactersStats;
using Pool;
using Zenject;
using System.Threading.Tasks;
using Gameplay;
using System.Collections.Generic;

namespace Enemy
{

    public interface IEnemyController : ICharacterController
    {
        public void OnClick(Transform point, PointerEventData eventData);
        public void Init(EnemyModel model, CharacterView playerView);
    }
    public class EnemyController : IEnemyController, IDisposable
    {
        private CharacterView _enemyView;
        private EnemyModel _enemyModel;
        private Rigidbody2D _enemyViewRigidbody;
        public event OnEndTurn OnEndTurn;
        private ICharacterScenarioContext _characterScenarioContext;
        public bool IsActive { get; set; }
        private ITestingBehaviourTree _testBehaviourTree;
        private List<IDisposable> _disposables;
        private DiContainer _container;
        private int _milisecondsDelay=3000;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }
        public void Init(EnemyModel enemyModel, CharacterView characterView)
        {
            _testBehaviourTree = _container.Resolve<ITestingBehaviourTree>();
            _testBehaviourTree.InitTree(this);
            _enemyModel = enemyModel;
            _enemyModel.Velocity.ToDisposableList(_disposables).Subscribe(EndTurn);
            _enemyView = characterView;
            _enemyView.Init(_enemyModel);
            _enemyView.ON_CLICK += OnClick;
        }
        public void OnClick(Transform point, PointerEventData eventData)
        {
            if (IsActive)
            {
                
            }
        }

        public void Dispose()
        {
            _enemyView.ON_CLICK -= OnClick;
        }

        public void Launch(Vector2 direction)
        {
            float launchPower = _enemyModel.GetStats().LaunchPower;
            direction.Normalize();
            Vector2 forceVector = direction * launchPower;
            _enemyView.AddImpulse(forceVector);
        }

        public void EndTurn(float velocity)
        {
            if (Mathf.Abs(velocity) < 0.2f && velocity != 0)
            {
                OnEndTurn?.Invoke();
                _enemyView.IsMoving = false;
            }
        }

        public async void Attack()
        {
            Transform target = _testBehaviourTree.GetTarget();
            Transform enemy = GetTransform();
            Vector2 direction = enemy.position - target.position;
            _enemyView.ChangeDirection(direction);
            await Task.Delay(_milisecondsDelay);
            Launch(direction * -1);
        }

        public async void Move()
        {
            Debug.Log("Enemy has moved");
            await Task.Delay(_milisecondsDelay);
        }

        public void Tick()
        {
            _testBehaviourTree.TickTree();
        }

        public void SetCharacterContext(ICharacterScenarioContext characterScenarioContext)
        {
            _characterScenarioContext = characterScenarioContext;
            _testBehaviourTree.SetCharacters(_characterScenarioContext);
        }

        public Transform GetTransform()
        {
            return _enemyView.GetTransform();
        }
    }
}
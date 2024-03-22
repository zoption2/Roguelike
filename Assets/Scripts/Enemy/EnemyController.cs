using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Zenject;
using CharactersStats;
using Interactions;
using System.Collections.Generic;
using Pool;

namespace Enemy
{
    public interface IEnemyView : ICharacterView, IMyPoolable
    {
        void InitEnemy(IControllerInputs controllerInputs);
    }

    public interface IEnemyController : ICharacterController
    {
        public void OnClick(Transform point, PointerEventData eventData);
    }
    public class EnemyController : IEnemyController, IControllerInputs, IDisposable
    {
        public event OnEndTurn ON_END_TURN;

        private IEnemyView _enemyView;
        private CharacterModel _enemyModel;
        private IEffector _effector;
        private CharacterPooler _pooler;

        private IInteractionProcessor _interactionProcessor;
        private IInteractionDealer _interactionDealer;
        public bool IsActive { get; set; }
        private Queue<IInteraction> _interactions;

        [Inject]
        public void Construct(
            IInteractionProcessor interactionProcessor,
            IInteractionDealer interactionDealer,
            IEffector effector
            )
        {
            _interactionProcessor = interactionProcessor;
            _interactionDealer = interactionDealer;
            _effector = effector;
        }

        public void Init(CharacterModel characterModel, CharacterView characterView, CharacterPooler characterPooler)
        {
            _enemyModel = characterModel;
            _enemyView = characterView;
            _pooler = characterPooler;
            _enemyView.InitEnemy(this);
            _enemyView.ON_CLICK += OnClick;
            _interactionProcessor.Init(_enemyModel, _effector);
        }

        public void OnClick(Transform point, PointerEventData eventData)
        {
            if (IsActive)
            {
                ON_END_TURN?.Invoke();
                Debug.Log(_enemyModel.Health);
                DoInteractionConclusion();
                Debug.Log(_enemyModel.Health);
            }
        }

        public Queue<IInteraction> GetInteractions()
        {
            return _interactions;
        }

        public void DoInteractionConclusion()
        {
            _enemyModel.Health = _enemyModel.ReactiveHealth.Value;
            if (_enemyModel.Health <= 0)
            {
                _pooler.Push(_enemyModel.Type, _enemyView);
            }
        }

        public void Dispose()
        {
            _enemyView.ON_CLICK -= OnClick;
        }

        public CharacterModel GetCharacterModel()
        {
            return _enemyModel;
        }

        public void ApplyInteractions(Queue<IInteraction> interactions)
        {
            _interactionProcessor.HandleInteraction(interactions);
        }
    }
}
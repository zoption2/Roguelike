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

    public interface IEnemyController : ICharacterController
    {
        public void OnClick(Transform point, PointerEventData eventData);
    }
    public class EnemyController : IEnemyController, IControllerInputs, IDisposable
    {
        public event OnEndTurn ON_END_TURN;

        private CharacterView _enemyView;
        private CharacterModel _enemyModel;
        private ModifiableStats _modifiableStats;
        private IEffector _effector;
        private CharacterPooler _pooler;

        private IInteractionProcessor _interactionProcessor;
        private IInteractionDealer _interactionDealer;
        public bool IsActive { get; set; }
        private IInteraction _interaction;

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

            _modifiableStats = new ModifiableStats(_enemyModel.GetStats());

            _enemyView = characterView;
            _pooler = characterPooler;
            _enemyView.Init(this);
            _enemyView.ON_CLICK += OnClick;
            _interactionProcessor.Init(_modifiableStats, _effector);
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

        public IInteraction GetInteraction()
        {
            return _interaction;
        }

        public void DoInteractionConclusion()
        {
            _enemyModel.Health = _modifiableStats.Health.Value;
            if (_enemyModel.Health <= 0)
            {
                _pooler.Push(_enemyModel.Type, _enemyView);
            }
        }

        public void Dispose()
        {
            _enemyView.ON_CLICK -= OnClick;
        }

        public ModifiableStats GetCharacterStats()
        {
            return _modifiableStats;
        }

        public void ApplyInteraction(IInteraction interaction)
        {
            _interactionProcessor.HandleInteraction(interaction);
        }

        public bool GetActiveStatus()
        {
            return IsActive;
        }
    }
}
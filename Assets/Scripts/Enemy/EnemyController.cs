﻿using Player;
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
        private IEffectProcessor _effector;
        private CharacterPooler _pooler;

        private IInteractionProcessor _interactionProcessor;
        private IInteractionDealer _interactionDealer;
        private IInteractionFinalizer _interactionFinalizer;
        public bool IsActive { get; set; }
        private IInteraction _interaction;

        [Inject]
        public void Construct(
            IInteractionProcessor interactionProcessor,
            IInteractionDealer interactionDealer,
            IEffectProcessor effector,
            IInteractionFinalizer interactionFinalizer)
        {
            _interactionProcessor = interactionProcessor;
            _interactionDealer = interactionDealer;
            _effector = effector;
            _interactionFinalizer = interactionFinalizer;
        }

        public void Init(CharacterModel characterModel, CharacterView characterView, CharacterPooler characterPooler)
        {
            _enemyModel = characterModel;

            _modifiableStats = new ModifiableStats(_enemyModel.GetStats());
            _interactionProcessor.Init(_effector);
            _interactionFinalizer.Init(_interactionProcessor, _effector);

            _enemyView = characterView;
            _pooler = characterPooler;
            _enemyView.Init(this);
            _enemyView.ON_CLICK += OnClick;
            _interactionProcessor.Init(_effector);
        }

        public void OnClick(Transform point, PointerEventData eventData)
        {
            if (IsActive)
            {
                _effector.ProcessEffectsOnStart(_modifiableStats);
                ON_END_TURN?.Invoke();
                _modifiableStats = _interactionFinalizer.FinalizeInteraction(_modifiableStats);
                DoInteractionConclusion();
            }
        }

        public IInteraction GetInteraction()
        {
            return _interaction;
        }

        public void DoInteractionConclusion()
        {
            Debug.LogWarning(_modifiableStats.Health.Value);
            if (_modifiableStats.Health.Value <= 0)
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
            if (!IsActive)
            {
                IEffect effect = interaction.GetEffect();
                if (effect != null)
                {
                    _effector.AddEffects(effect);
                }
                _interactionProcessor.ProcessInteraction(interaction);
            }
        }
    }
}
using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Interactions;
using System.Collections.Generic;
using Pool;
using BehaviourTree;
using CharactersStats;
using Zenject;
using System.Threading.Tasks;
using Gameplay;
using Prefab;

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
        private ReactiveStats _reactiveStats;
        private IEffectProcessor _effector;
        private CharacterPooler _pooler;
        private ICharacterScenarioContext _characterScenarioContext;
        private ITestingBehaviourTree _testBehaviourTree;
        private IInteractionProcessor _interactionProcessor;
        private IInteractionDealer _interactionDealer;
        private IInteractionFinalizer _interactionFinalizer;
        private ReactiveStats _interactionResult;
        public bool IsActive { get; set; }
        private List<IDisposable> _disposables;
        private IInteraction _interaction;
        private DiContainer _container;
        private int _milisecondsDelay = 3000;

        [Inject]
        public void Construct(
            IInteractionProcessor interactionProcessor,
            IInteractionDealer interactionDealer,
            IEffectProcessor effector,
            IInteractionFinalizer interactionFinalizer,
            DiContainer container)
        {
            _interactionProcessor = interactionProcessor;
            _interactionDealer = interactionDealer;
            _effector = effector;
            _interactionFinalizer = interactionFinalizer;
            _container = container;
        }

        public void Init(CharacterModel characterModel, CharacterView characterView, CharacterPooler characterPooler)
        {
            _testBehaviourTree = _container.Resolve<ITestingBehaviourTree>();
            _testBehaviourTree.InitTree(this);

            _enemyModel = characterModel;

            var stats = _enemyModel.GetStats();
            _reactiveStats = stats.ToReactive();

            _reactiveStats.Velocity.ToDisposableList(_disposables).Subscribe(EndTurn);

            _interactionDealer.Init(_reactiveStats);
            _interactionProcessor.Init(_effector);

            _enemyView = characterView;
            _pooler = characterPooler;
            _enemyView.Init(this);
            _enemyView.ON_CLICK += OnClick;
        }

        public void OnClick(Transform point, PointerEventData eventData)
        {
            Debug.LogWarning("Effects on Start interaction: \n");
            _effector.PrintEffects(_effector.GetOnStartTurnInteractionEffects());

            Debug.LogWarning("Effects on End interaction: \n");
            _effector.PrintEffects(_effector.GetOnEndTurnInteractionEffects());

            //if (IsActive)
            //{
            //    _effector.ProcessEffectsOnStart(_reactiveStats);
            //}
        }

        public IInteraction GetInteraction()
        {

            //////////////////////|Check effects before interaction|\\\\\\\\\\\\\\\\\\\\\
            _effector.ProcessStatsBeforeInteraction(_reactiveStats);
            //////////////////////|--------------------------------|\\\\\\\\\\\\\\\\\\\\\

            _interactionDealer.Init(_reactiveStats);
            IInteraction interaction = _interactionDealer.UseInteraction(InteractionType.BasicAttack);
            return interaction;
        }

        public void PushIfDead()
        {
            Debug.LogWarning("Hp After Interaction: " + _reactiveStats.Health.Value);
            if (_reactiveStats.Health.Value <= 0)
            {
                _pooler.Push(_enemyModel.Type, _enemyView);
            }
        }

        public void Dispose()
        {
            _enemyView.ON_CLICK -= OnClick;
        }

        public ReactiveStats GetCharacterStats()
        {
            return _reactiveStats;
        }

        public void ApplyInteraction(IInteraction interaction)
        {
            if (!IsActive)
            {
                List<IEffect> effects = interaction.GetEffects();
                if (effects != null && effects.Count > 0)
                {
                    foreach (IEffect effect in effects)
                    {
                        _effector.AddEffects(effects);
                    }
                }
                _interactionResult = _interactionProcessor.ProcessInteraction(interaction);

            }
        }

        public void AddEffects(List<IEffect> effects)
        {
            if (effects != null && effects.Count > 0)
            {
                foreach (IEffect effect in effects)
                {
                    _effector.AddEffects(effects);
                }
            }
        }
        public void Launch(Vector2 direction)
        {
            float launchPower = _reactiveStats.LaunchPower.Value;
            direction.Normalize();
            Vector2 forceVector = direction * launchPower;
            _enemyView.AddImpulse(forceVector);
        }

        public void EndTurn(float velocity)
        {
            if (Mathf.Abs(velocity) < 0.2f && velocity != 0)
            {
                ON_END_TURN?.Invoke();
                _enemyView.IsMoving = false;
                Debug.LogWarning("Hp Before Interaction: " + _reactiveStats.Health.Value);
                if (_interactionResult != null)
                {
                    _reactiveStats = _interactionFinalizer.FinalizeInteraction(_reactiveStats, _interactionResult);


                }
                PushIfDead();

            }
        }



        public async void Attack()
        {
            Transform target = _testBehaviourTree.GetTarget();
            Transform enemy = GetTransform();
            Vector2 direction = enemy.position - target.position;
            _enemyView.ChangeDirection(-direction);
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
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
        private ICharacterScenarioContext _characterScenarioContext;
        private ITestingBehaviourTree _testBehaviourTree;
        private IInteractionProcessor _interactionProcessor;
        private IInteractionDealer _interactionDealer;
        private IInteractionFinalizer _interactionFinalizer;
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

            _modifiableStats.Velocity.ToDisposableList(_disposables).Subscribe(EndTurn);

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
            Debug.LogWarning("Effects on Start interaction: \n");
            _effector.PrintEffects(_effector.GetOnStartTurnInteractionEffects());

            Debug.LogWarning("Effects on End interaction: \n");
            _effector.PrintEffects(_effector.GetOnEndTurnInteractionEffects());

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
            Debug.LogWarning("Hp After Interaction: " + _modifiableStats.Health.Value);
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
                List<IEffect> effects = interaction.GetEffects();
                if (effects != null && effects.Count > 0)
                {
                    foreach (IEffect effect in effects)
                    {
                        _effector.AddEffects(effects);
                    }
                }
                _interactionProcessor.ProcessInteraction(interaction);
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
                float launchPower = _modifiableStats.LaunchPower.Value;
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
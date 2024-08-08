using UnityEngine;
using UnityEngine.EventSystems;
using Interactions;
using System.Collections.Generic;
using BehaviourTree;
using CharactersStats;
using Zenject;
using Gameplay;
using UnityEngine.AI;
using Abilities;
using Projectiles;

namespace Enemy
{
    public interface IEnemyController : ICharacterController
    {
        void DisactivateAbilityPanel();
        public AnimationBase AttackAnimation { get; set; }
    }

    public class EnemyController : CharacterControllerBase, IEnemyController
    {
        public AnimationBase AttackAnimation {  get; set; }
        public AnimationBase MoveAnimation { get; set; }

        public EnemyController(
            IPoolManager poolManager,
            IInteractionProcessor interactionProcessor,
            IInteractionDealer interactionDealer,
            IEffectProcessor effector,
            IInteractionCalculator interactionFinalizer,
            IStateFactory stateFactory,
            IUIFactory uIFactory,
            DiContainer container,
            IGameplayService gameplayService,
            ICameraManager cameraManager)
            : base(poolManager, interactionProcessor, interactionDealer, effector, interactionFinalizer, stateFactory, uIFactory, container, gameplayService, cameraManager)
        {
        }

        public override void Init(
            CharacterModel characterModel,
            CharacterView characterView,
            CharacterUIView characterUIView)
        {
            DefaultBehaviourTree = Container.Resolve<IDefaultBehaviourTree>();
            DefaultBehaviourTree.InitTree(this);
            DefaultBehaviourTree.SetAbilities(characterModel.Abilities);

            CharacterModel = characterModel;

            var stats = CharacterModel.GetStats();
            ModifiableStats = stats.ToReactive();

            AllEffects = CharacterModel.GetAllEffects();

            CharacterView = characterView;
            CharacterView.Init(this);

            CurrentState = StateFactory.CreateConditionState(TypeOfConditionState.InactiveState, this);
            Analyzer = new Analyzer(this);

            InteractionDealer.Init(ModifiableStats);

            UIView = characterUIView;

            UIViewmodel = new CharacterUIViewmodel();
            UIViewmodel.Init(CharacterModel, UIFactory, UIView, this);

            UIView.Init(CharacterView, UIViewmodel);
            Effector.Init(UIViewmodel, AllEffects);
            UIViewmodel.UpdateReloadIndicators();

            NavMeshAgent = CharacterView.NavMeshAgent;
            NavMeshAgent.updateUpAxis = false;
            NavMeshAgent.updateRotation = false;
            NavMeshAgent.enabled = false;
            CharacterView.ON_CLICK += OnClick;
            ON_STOP_MOVEMENT += CheckForEndOfState;

            NavMeshObstacle = CharacterView.NavMeshObstacle;
            NavMeshObstacle.carving = true;
            NavMeshObstacle.carveOnlyStationary = true;

            LaunchedProjectiles = new List<IProjectile>();

            AttackAnimation = new EnemyAttackAnimation(this);
            MoveAnimation = new MoveAnimation(this);
        }

        public override void SetCurrentAbility(IAbility ability)
        {
        }

        public override void OnClick(Transform point, PointerEventData eventData)
        {
            SlingShotInitPosition = point;
            bool isPlayerTurn = true;
            ICharacterController activePlayer = CharacterScenarioContext.Players[0];

            foreach (ICharacterController enemy in CharacterScenarioContext.Enemies)
            {
                if (enemy.IsActive)
                {
                    isPlayerTurn = false;
                    break;
                }
            }

            foreach (ICharacterController player in CharacterScenarioContext.Players)
            {
                if (player.IsActive)
                {
                    activePlayer = player;
                }
            }

            if (isPlayerTurn && !activePlayer.IsMoving && activePlayer.LaunchedProjectiles.Count == 0)
            {
                UIViewmodel.ActivateSkillsBTNs();
            }
        }

        public void DisactivateAbilityPanel()
        {
            UIViewmodel.DeactivateSkillsBTNs();
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (IsActive && !IsMoving)
            {
                CurrentState.UseSlingshotAsync(eventData, SlingShotInitPosition);
            }
        }

        public override void ProcessReloadAbility()
        {
        }

        public override void RevertReadyUnactiveAbilityButtons()
        {
        }

        public override void ProcessOnEndTurn()
        {
            base.ProcessOnEndTurn();
            foreach (IAbility ability in CharacterModel.Abilities)
            {
                ability.TickReload();
            }
            UIViewmodel.UpdateReloadIndicators();
        }
    }
}

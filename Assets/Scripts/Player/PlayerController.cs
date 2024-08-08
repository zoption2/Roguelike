using CharactersStats;
using Interactions;
using Gameplay;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using UnityEngine.AI;
using BehaviourTree;
using System.Linq;
using Enemy;
using Abilities;
using Projectiles;

namespace Player
{
    public interface IPlayerController : ICharacterController
    {
        void SetTransform(Transform newTransform);
        void StopPlayer();
    }

    public class PlayerController : CharacterControllerBase, IPlayerController
    {
        private List<IAbility> AbilitiesForReload;
        private IAbility BasicAbility;

        public PlayerController(
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
            SlingShotPooler = poolManager.UseSlingshotPooler();
        }

        public override void Init(
            CharacterModel playerModel,
            CharacterView characterView,
            CharacterUIView characterUIView)
        {
            if (DefaultBehaviourTree == null)
            {
                DefaultBehaviourTree = Container.Resolve<IDefaultBehaviourTree>();
            }
            DefaultBehaviourTree.InitTree(this);
            DefaultBehaviourTree.SetAbilities(playerModel.Abilities);

            CharacterModel = playerModel;

            var stats = CharacterModel.GetStats();
            ModifiableStats = stats.ToReactive();

            AllEffects = CharacterModel.GetAllEffects();

            CharacterView = characterView;
            CharacterView.Init(this);

            CurrentState = StateFactory.CreateConditionState(TypeOfConditionState.InactiveState, this);
            Analyzer = new Analyzer(this);

            UIView = characterUIView;

            UIViewmodel = new CharacterUIViewmodel();
            UIViewmodel.Init(CharacterModel, UIFactory, UIView, this);

            UIView.Init(CharacterView, UIViewmodel);
            Effector.Init(UIViewmodel, AllEffects);
            UIViewmodel.UpdateReloadIndicators();

            NavMeshAgent = CharacterView.NavMeshAgent;
            NavMeshAgent.enabled = false;
            NavMeshObstacle = CharacterView.NavMeshObstacle;
            NavMeshObstacle.carving = true;
            NavMeshObstacle.carveOnlyStationary = true;

            CharacterView.ON_CLICK += OnClick;
            CharacterView.ON_BEGINDRAG += OnBeginDrag;
            ON_STOP_MOVEMENT += CheckForEndOfState;

            foreach (var ability in CharacterModel.Abilities)
            {
                if (ability is BasicAttackAbility)
                {
                    BasicAbility = ability;
                }
            }

            CurrentAbility = BasicAbility;

            LaunchedProjectiles = new List<IProjectile>();

            AnimationController = new PlayerAnimationController();
            AnimationController.SetCharacter(this);
        }

        public override void SetCurrentAbility(IAbility ability)
        {
            CurrentAbility = ability;
            Debug.LogWarning(CurrentAbility + "  " + CurrentAbility.ProjectileType);
        }

        public override void OnClick(Transform point, PointerEventData eventData)
        {
            SlingShotInitPosition = point;
            if (!IsMoving && IsActive)
            {
                UIViewmodel.ActivateSkillsBTNs();
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            UIViewmodel.DeactivateSkillsBTNs();
            DisableEnemiesSkillButtons();

            if (!IsMoving && LaunchedProjectiles.Count == 0)
            {
                CurrentState.UseSlingshotAsync(eventData, SlingShotInitPosition);
            }
        }

        private void DisableEnemiesSkillButtons()
        {
            foreach (IEnemyController enemy in CharacterScenarioContext.Enemies)
            {
                enemy.DisactivateAbilityPanel();
            }
        }

        public override void ProcessReloadAbility()
        {
            CurrentAbility.UseAbility();
            if (!CurrentAbility.ReadyForUse)
            {
                UIViewmodel.ChangeButtonInteractible(CurrentAbility, false);
            }
        }

        public override void ProcessOnStartTurn()
        {
            base.ProcessOnStartTurn();
            AbilitiesForReload = CharacterModel.Abilities.Where(x => !x.ReadyForUse).ToList();
            foreach (IAbility ability in CharacterModel.Abilities)
            {
                ability.TickReload();
            }
            UIViewmodel.UpdateReloadIndicators();
            CurrentAbility = BasicAbility;
        }

        public override void RevertReadyUnactiveAbilityButtons()
        {
            foreach (IAbility ability in CharacterModel.Abilities)
            {
                if (ability.ReadyForUse)
                {
                    UIViewmodel.ChangeButtonInteractible(ability, true);
                }
            }
        }

        public override void ProcessOnEndTurn()
        {
            base.ProcessOnEndTurn();
            if (AbilitiesForReload.Count > 0)
            {
                foreach (IAbility ability in AbilitiesForReload)
                {
                    if (ability.ReadyForUse)
                    {
                        UIViewmodel.ChangeButtonInteractible(ability, true);
                    }
                }
            }

            DisableEnemiesSkillButtons();
            RevertReadyUnactiveAbilityButtons();
        }

        public void SetTransform(Transform newTransform)
        {
            CharacterView.SetTransform(newTransform);
        }

        public void StopPlayer()
        {
            Rigidbody rb = CharacterView.GetRigidbody();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}

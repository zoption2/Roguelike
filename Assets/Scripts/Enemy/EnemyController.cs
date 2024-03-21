using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Zenject;
using CharactersStats;
using Interactions;

namespace Enemy
{

    public interface IEnemyController : ICharacterController
    {
        public void OnClick(Transform point, PointerEventData eventData);
    }
    public class EnemyController : IEnemyController, IDisposable
    {
        private CharacterView _enemyView;
        private CharacterModel _enemyModel;
        private Rigidbody2D _enemyViewRigidbody;
        private Stats _enemyStats;
        public event OnEndTurn ON_END_TURN;

        IInteractionProcessor _interactionProcessor;
        public bool IsActive { get; set; }

        [Inject]
        public void Construct(
            IInteractionProcessor interactionProcessor
            )
        {
            _interactionProcessor = interactionProcessor;
        }

        public void Init(CharacterModel characterModel, CharacterView characterView)
        {
            _enemyModel = characterModel;
            _enemyView = characterView;
            _enemyView.Init(_enemyModel);
            _enemyStats = _enemyModel.GetStats();
            _enemyView.ON_CLICK += OnClick;
            _enemyView.ON_INTERACTION_FINISH += ApplyStats;
        }
        public void OnClick(Transform point, PointerEventData eventData)
        {
            if (IsActive)
            {
                //Debug.Log("Enemy was clicked");
                ON_END_TURN?.Invoke();
            }
        }

        public void ApplyStats(Stats updatetInteractionHandlerStats)
        {
            //Debug.LogWarning($"Handler Stats before interaction. Healt: {_enemyStats.Health}");
            _enemyModel.ReactiveHealth.Value = updatetInteractionHandlerStats.Health;
            _enemyStats.Health = _enemyModel.ReactiveHealth.Value;
            //Debug.LogWarning($"Handler Stats after interaction. Healt: {_enemyStats.Health}");
        }

        public void Dispose()
        {
            _enemyView.ON_CLICK -= OnClick;
            _enemyView.ON_INTERACTION_FINISH -= ApplyStats;
        }
    }
}
using Player;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

namespace Enemy
{
    public interface IEnemyController : ICharacterController
    {

    }
    public class EnemyController : IEnemyController
    {
        private EnemyView _enemyView;
        private EnemyModel _enemyModel;
        public delegate void OnSwitchState();
        public static event OnSwitchState OnSwitch;
        public bool IsActive { get; set; }

        //TODO: add methods.
        public virtual void Init()
        {
            _enemyView = GameObject.Find("Enemy").GetComponent<EnemyView>();
            _enemyView.Initialize(this);
            _enemyModel = new EnemyModel();
        }
        public void Init(EnemyView enemyView, EnemyModel enemyModel)
        {
            _enemyView = enemyView;
            _enemyView.Initialize(this);
            _enemyModel = enemyModel;
        }
        public void OnClick()
        {
            if (IsActive)
            {
                Debug.Log("Enemy was clicked");
                OnSwitch?.Invoke();
            }
        }
    }
}
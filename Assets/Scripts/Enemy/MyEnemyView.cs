using Player;
using Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Enemy
{
    public interface IEnemyView
    {
        public void Initialize(EnemyController enemyController);
    }
    public class MyEnemyView : MonoBehaviour, IEnemyView, IMyPoolable ,IPointerDownHandler
    {
        private EnemyController _enemyController;

        public void Initialize(EnemyController enemyController)
        {
            _enemyController = enemyController;
        }

        public void Lauch(float power, Vector2 direction)
        {
            Debug.Log("Nothing happened yet");
        }

        public void OnCreate()
        {

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _enemyController.OnClick();
        }

        public void OnPull()
        {

        }

        public void OnRelease()
        {

        }
    }
}

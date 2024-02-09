using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Enemy
{
    public class EnemyView : MonoBehaviour,IPointerDownHandler
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

        public void OnPointerDown(PointerEventData eventData)
        {
            _enemyController.OnClick();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obstacles
{
    public class TeleportWallEnter : MonoBehaviour
    {
        [SerializeField] Transform ExitPoint;

        public void ProcessCollision(Rigidbody rigidbody)
        {
            Vector3 velocity = rigidbody.velocity;

            // Телепортувати об'єкт до точки виходу
            rigidbody.transform.position = ExitPoint.position;

            // Відновити напрямок та швидкість об'єкта
            rigidbody.velocity = velocity;

            // Викликати метод вилітання з телепорту через 1 секунду
            Invoke(nameof(ExitTeleport), 1f);
        }

        private void ExitTeleport()
        {
            if (ExitPoint.TryGetComponent(out TeleportWallExit teleportExit))
            {
                teleportExit.ExitObject(); // Викликаємо метод виходу з телепорту
            }
        }

    }
}
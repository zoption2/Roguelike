using UnityEngine;

namespace Obstacles
{
    public class TeleportWallExit : MonoBehaviour
    {
        public void ExitObject()
        {
            if (GetComponent<Rigidbody>() != null)
            {
                // Вивільнення об'єкта з телепорту
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero; // Стоп швидкість
                rb.angularVelocity = Vector3.zero; // Стоп обертання
            }
        }
    }
}

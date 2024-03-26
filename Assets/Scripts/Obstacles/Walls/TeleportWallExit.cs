using UnityEngine;

namespace Obstacles
{
    public class TeleportWallExit : MonoBehaviour
    {
        public void ExitObject()
        {
            if (GetComponent<Rigidbody>() != null)
            {
                // ���������� ��'���� � ���������
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero; // ���� ��������
                rb.angularVelocity = Vector3.zero; // ���� ���������
            }
        }
    }
}

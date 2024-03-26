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

            // ������������� ��'��� �� ����� ������
            rigidbody.transform.position = ExitPoint.position;

            // ³������� �������� �� �������� ��'����
            rigidbody.velocity = velocity;

            // ��������� ����� �������� � ��������� ����� 1 �������
            Invoke(nameof(ExitTeleport), 1f);
        }

        private void ExitTeleport()
        {
            if (ExitPoint.TryGetComponent(out TeleportWallExit teleportExit))
            {
                teleportExit.ExitObject(); // ��������� ����� ������ � ���������
            }
        }

    }
}
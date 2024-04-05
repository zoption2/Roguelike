using UnityEngine;

namespace Obstacles
{
    public class TeleportWallEnter : MonoBehaviour
    {
        [SerializeField] private GameObject exit;
        [SerializeField] private int _teleportationsCountPerTurn;
        [SerializeField] private Material EnterEnabledMaterial;
        [SerializeField] private Material EnterDisabledMaterial;
        [SerializeField] private Material ExitEnabledMaterial;
        [SerializeField] private Material ExitDisabledMaterial;
        private BoxCollider _collider;
        private Renderer _renderer;
        private Renderer _exitRenderer;
        private int _teleportationsCount;

        private void Start()
        {
            _teleportationsCount = _teleportationsCountPerTurn;
            _collider = GetComponent<BoxCollider>();
            _renderer = GetComponent<Renderer>();
            _exitRenderer = exit.GetComponent<Renderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out ICharacterView character) && _teleportationsCount > 0)
            {
                Vector3 velocity = other.GetComponent<Rigidbody>().velocity;

                other.transform.position = exit.transform.position;

                other.GetComponent<Rigidbody>().velocity = velocity;

                _teleportationsCount--;
            }
            else if (_teleportationsCount <= 0)
            {
                _collider.enabled = false;

                //_exitRenderer.material.SetMaterial(ExitDisabledMaterial);
                //_renderer.material.SetMaterial(0, EnterDisabledMaterial);
            }
        }

        public void Recharge()
        {
            _teleportationsCount = _teleportationsCountPerTurn;
            _collider.enabled = true;

            //_exitRenderer.material.SetMaterial(0, ExitEnabledMaterial);
            //_renderer.material.SetMaterial(0, EnterEnabledMaterial);
        }
    }
}

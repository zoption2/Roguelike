using Obstacles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectile
{
    public interface IProjectileCollisionHandler
    {
        void Init(IControllerInputs controllerInputs, IProjectile projectile);
    }
    public class ProjectileCollisionHandler : MonoBehaviour
    {
        private IProjectile _projectile;
        private Rigidbody _rigidbody;

        public void Init(IProjectile projectile)
        {
            _projectile = projectile;
            _rigidbody = _projectile.GetRigidbody();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out IInteractible interactible))
            {
                _projectile.Normal = collision.GetContact(0).normal;
                _projectile.StartInteraction(interactible);
            }
            _projectile.PushToPool();
        }
    }
}

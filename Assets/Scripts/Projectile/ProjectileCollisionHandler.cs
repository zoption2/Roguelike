using Interactions;
using Obstacles;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Projectiles
{
    public interface IProjectileCollisionHandler
    {
        void Init(IControllerInputs controllerInputs, IProjectile projectile);
        public void SetRicochetCount(int count);
    }
    public class ProjectileCollisionHandler : MonoBehaviour
    {
        private IProjectile _projectile;
        private Rigidbody _rigidbody;
        private int _ricochetCount;

        public void Init(IProjectile projectile)
        {
            _projectile = projectile;
            _rigidbody = _projectile.GetRigidbody();
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        public void SetRicochetCount(int count)
        {
            _ricochetCount = count;
        }

        private void CheckRichochet(IWall obstacle)
        {
            if (obstacle is StickyWall)
            {
                _projectile.ControllerInputs.HandleStopMovement();
                _projectile.PushToPool();
            }
            else if (_ricochetCount == 0)
            {
                _projectile.ControllerInputs.HandleStopMovement();
                _projectile.PushToPool();
            }
            else
                _ricochetCount--;
        }

        private void OnCollisionEnter(Collision collision)
        {
            bool InteractibleIsAlive = true;

            if (collision.gameObject.TryGetComponent(out IWall obstacle))
            {
                CheckRichochet(obstacle);
                Vector3 velocity = _projectile.GetLastVelocity();

                obstacle.ProcessCollision(collision, _rigidbody, velocity);
            }

            if (collision.gameObject.TryGetComponent(out IInteractible interactible))
            {
                _projectile.Normal = collision.GetContact(0).normal;
                _projectile.PushToPool();
                _projectile.StartInteraction(interactible);
                InteractibleIsAlive = interactible.GetRigidbody().gameObject.activeInHierarchy;
            }

            if (!InteractibleIsAlive)
            {
                _projectile.ControllerInputs.HandleStopMovement();
            }
        }

        private Vector3 GetCurrentVelocity()
        {
            return _rigidbody.velocity;
        }
    }
}

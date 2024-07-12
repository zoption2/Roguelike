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
        private int _currentCollisions = 0;

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
                _projectile.PushToPool();
            }
            else if (_ricochetCount == 0)
            {
                _projectile.PushToPool();
            }
            else
                _ricochetCount--;
        }

        private void OnCollisionEnter(Collision collision)
        {
            _currentCollisions++;

            if(_currentCollisions >= 4)
            {
                _projectile.PushToPool();
            }

            if (collision.gameObject.TryGetComponent(out IWall obstacle))
            {
                Vector3 velocity = _projectile.GetLastVelocity();
                obstacle.ProcessCollision(collision, _rigidbody, velocity);
                CheckRichochet(obstacle);
            }

            if (collision.gameObject.TryGetComponent(out IChestView chest))
            {
                _projectile.PushToPool();
            }

            if (collision.gameObject.TryGetComponent(out IInteractible interactible))
            {
                _projectile.Normal = collision.GetContact(0).normal;
                _projectile.StartInteraction(interactible);
                _projectile.PushToPool();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            _currentCollisions--;
        }

        private Vector3 GetCurrentVelocity()
        {
            return _rigidbody.velocity;
        }

        public void ResetCollisions()
        {
            _currentCollisions = 0;
        }
    }
}

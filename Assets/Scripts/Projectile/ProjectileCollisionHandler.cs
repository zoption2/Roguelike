using Interactions;
using Obstacles;
using System.Collections;
using System.Collections.Generic;
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
        private bool _hadCollisionInThisFrame;

        [ContextMenu("TestStop")]
        private void TESTSTOP()
        {
            _rigidbody.velocity = Vector3.zero;
        }

        [ContextMenu("TestLaunch")]
        private void TESTLAUNCH()
        {
            Vector3 p = new Vector3(5, 0, 0);
            _rigidbody.AddForce(p, ForceMode.Impulse);
        }

        public void Init(IProjectile projectile)
        {
            _projectile = projectile;
            _rigidbody = _projectile.GetRigidbody();
        }

        private void FixedUpdate()
        {
            _hadCollisionInThisFrame = false;
        }

        public void SetRicochetCount(int count)
        {
            //_ricochetCount = count;
            _ricochetCount = 999;
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
            if (_hadCollisionInThisFrame)
                return;

            bool InteractibleIsAlive = true;

            if (collision.gameObject.TryGetComponent(out IWall obstacle))
            {
                _hadCollisionInThisFrame = true;
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
    }
}

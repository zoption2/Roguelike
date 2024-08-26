using System.Collections;
using UnityEngine;

namespace Obstacles
{
    public class DistortionWall : MonoBehaviour, IWall
    {
        private ParticlePooler _pooler;
        WallParticles _particles;


        public void ProcessCollision(Collision collision, Rigidbody rigidbody, Vector3 velocity, ParticlePooler particlePooler = null)
        {
            _pooler = particlePooler;

            Vector3 reflectionVector = collision.GetContact(0).normal;

            Vector3 reflectedVelocity = Vector3.Reflect(new Vector3(velocity.x, 0, velocity.z), reflectionVector);
            rigidbody.velocity = reflectedVelocity;

            if (reflectedVelocity != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(reflectedVelocity);
                rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, targetRotation, Time.deltaTime * 10f);
            }

            ContactPoint contactPoint = collision.GetContact(0);

            _particles = _pooler.Pull<WallParticles>(ParticleType.WallParticle, contactPoint.point, Quaternion.identity);

            _particles.transform.rotation = Quaternion.LookRotation(-velocity);

            _particles.Play();

            StartCoroutine(DestroyParticlesAfterCompletion());
        }

        private IEnumerator DestroyParticlesAfterCompletion()
        {
            while (_particles.Particles.IsAlive(true))
            {
                yield return null;
            }

            Debug.LogWarning("Push!!!");

            _pooler.Push(ParticleType.WallParticle, _particles);
        }
    }
}

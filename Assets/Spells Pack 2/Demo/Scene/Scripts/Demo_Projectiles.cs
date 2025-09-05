using UnityEngine;
using UnityEngine.Playables;

namespace ZakhanSpellsPack2
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]

    public class Demo_Projectiles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] InfiniteParticles;
        [SerializeField] private ParticleSystem[] LoopingParticles;
        [SerializeField] private TrailRenderer[] TrailRenderers;
        [SerializeField] private PlayableDirector ExplosionDirector;

        private void Awake()
        {
            gameObject.layer = 2; // Ignore Raycast to avoid hitting the camera
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(ExplosionDirector)
                ExplosionDirector.Play();

            if (InfiniteParticles.Length > 0) {
                foreach (ParticleSystem IP in InfiniteParticles)
                {
                    IP.Clear(false);
                    IP.Stop(false);
                }
            }
            if (LoopingParticles.Length > 0) {
                foreach (ParticleSystem LP in LoopingParticles)
                {
                    var emission = LP.emission;
                    emission.enabled = false;
                }
            }
            if (TrailRenderers.Length > 0) {
                foreach (TrailRenderer TR in TrailRenderers)
                {
                    TR.emitting = false;
                }
            }
        }
        private void OnDisable()
        {
            //Reset
            if (LoopingParticles.Length > 0) {
                foreach (ParticleSystem LP in LoopingParticles)
                {
                    var emission = LP.emission;
                    emission.enabled = true;
                }
            }
            if (TrailRenderers.Length > 0) {
                foreach (TrailRenderer TR in TrailRenderers)
                {
                    TR.emitting = true;
                }
            }
        }

    }

}



using UnityEngine;
using UnityEngine.VFX;

namespace ZakhanSpellsPack2
{
	public class SP2_VFXDecalTrail : MonoBehaviour
	{
		[SerializeField] private ParticleSystem Particle;
		[SerializeField] private float EmissionRate;
		private float NextEmission = 0f;

		[SerializeField] private VisualEffect VFXGraph;
		[SerializeField] private string AttributeName;
		[SerializeField] private string EventName;

		private ParticleSystem.Particle[] particles;

		void Start()
		{
			particles = new ParticleSystem.Particle[Particle.main.maxParticles];
		}

		private int Selection = 0;
		void LateUpdate()
		{
			if(!Particle.isEmitting) { return; }

			int numParticlesAlive = Particle.GetParticles(particles);

			if (Time.time > NextEmission)
			{
				if (Selection < numParticlesAlive)
				{
					Vector3 particlePosition = particles[Selection].position;
					var eventAttribute = VFXGraph.CreateVFXEventAttribute();
					eventAttribute.SetVector3(AttributeName, particlePosition);
					VFXGraph.SendEvent(EventName, eventAttribute);

					NextEmission = Time.time + EmissionRate;
					Selection++;
				}

				if(Selection > numParticlesAlive - 1)
				{
					Selection = 0;
				}
			}
		}
	}
}

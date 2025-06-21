using UnityEngine;

public class FireAOESkill : BaseSkill
{
    [SerializeField] private ParticleSystem _aoeParticles;
    public override void PerformSkill(GameObject player)
    {
        _aoeParticles.transform.position = player.transform.position;
        _aoeParticles.Play();
    }
}

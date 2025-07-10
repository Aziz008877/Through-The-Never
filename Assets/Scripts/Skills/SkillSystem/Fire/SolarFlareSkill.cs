using UnityEngine;

public class SolarFlareSkill : ActiveSkillBehaviour
{
    [Header("Feedback")]
    [SerializeField] private ParticleSystem _activateVfx;
    [SerializeField] private AudioSource _activateSfx;
    public override void TryCast()
    {
        if (!IsReady) return;
        if (PlayerContext.SolarFlareCharge) return;

        PlayerContext.SolarFlareCharge = true;

        if (_activateVfx) _activateVfx.Play();
        if (_activateSfx) _activateSfx.Play();

        StartCooldown();
    }
}
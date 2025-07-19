using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public sealed class CameraShakeEffect : MonoBehaviour
{
    [SerializeField] private float _magnitude = 1f;
    [SerializeField] private float _roughness = 2f;
    private CinemachineImpulseSource _impulse;
    private void Awake() => _impulse = GetComponent<CinemachineImpulseSource>();
    
    public void ShakeCamera(float impulseForce)
    {
        float impulse = Mathf.Clamp(impulseForce * 0.05f, .2f, _magnitude);
        _impulse.GenerateImpulseWithVelocity(Random.onUnitSphere * impulse * _roughness);
    }
}


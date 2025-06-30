using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource _cinemachineImpulseSource;

    public void Shake()
    {
        _cinemachineImpulseSource.GenerateImpulse();
    }

    public void ShakeWithDuration()
    {
        
    }
}

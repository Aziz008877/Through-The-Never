using System.Collections;
using UnityEngine;
public class RootTraps : MonoBehaviour
{
    [SerializeField] private ParticleSystem _rootVFX;
    [SerializeField] private float _rootDuration;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            
        }
        
        if (other.TryGetComponent(out ActorContext context))
        {
            
        }
    }

    private IEnumerator ActivateRoot()
    {
        yield return new WaitForSeconds(_rootDuration);
    }
}

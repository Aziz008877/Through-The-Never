using System.Collections;
using DG.Tweening;
using UnityEngine;
public class RootTrap : MonoBehaviour
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
            Vector3 pos = new Vector3(_rootVFX.transform.position.x, context.transform.position.y, _rootVFX.transform.position.z);
            context.transform.DOMove(pos, 0.5f);
            StartCoroutine(ActivateRootPlayer(context));
        }
    }

    private IEnumerator ActivateRootPlayer(ActorContext context)
    {
        _rootVFX.Play();
        context.State.ChangePlayerState(false);
        yield return new WaitForSeconds(_rootDuration);
        context.State.ChangePlayerState(true);
    }
}

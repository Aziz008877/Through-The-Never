using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public sealed class HitHighlightFeedbackEffect : MonoBehaviour
{
    [SerializeField] private float _duration = .2f;
    [ColorUsage(true, true)]
    [SerializeField] private Color _flashColor = Color.white;

    public void EnemyReceivedDamage(IDamageable target)
    {
        if (target is MonoBehaviour monobeh && monobeh.TryGetComponent(out Renderer renderer))
        {
            StartCoroutine(Flash(renderer));
        }
    }

    private IEnumerator Flash(Renderer rend)
    {
        var mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb);
        mpb.SetColor("_EmissionColor", _flashColor);
        rend.SetPropertyBlock(mpb);

        yield return new WaitForSeconds(_duration);
        
        mpb.SetColor("_EmissionColor", Color.black);
        rend.SetPropertyBlock(mpb);
    }
}
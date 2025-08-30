using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
public class CollisionSpikes : MonoBehaviour
{
    [SerializeField] private Spikes _spikes;
    [SerializeField] private float _yPosition, _yStartPosition, _spikesActiveTime;
    [SerializeField] private DotweenSettings _dotweenSettings;
    private bool _isActive = false;
    private void OnTriggerEnter(Collider other)
    {
        if (_isActive) return;
        
        if (other.TryGetComponent(out IDamageable damageable))
        {
            StartCoroutine(ActivateSpikes());
        }

        if (other.TryGetComponent(out ActorContext context))
        {
            StartCoroutine(ActivateSpikes());
        }
    }

    private IEnumerator ActivateSpikes()
    {
        if (!_isActive)
        {
            yield return new WaitForSeconds(1);
            _isActive = true;
        
            _spikes.transform
                .DOLocalMoveY(_yPosition, _dotweenSettings.Duration)
                .SetEase(_dotweenSettings.AnimationType);

            yield return new WaitForSeconds(_spikesActiveTime);

            _spikes.transform
                .DOLocalMoveY(_yStartPosition, _dotweenSettings.Duration)
                .SetEase(_dotweenSettings.AnimationType)
                .OnComplete(delegate
                {
                    _isActive = false;
                });
        }
    }
}

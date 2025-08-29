using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PassiveSpikes : MonoBehaviour
{
    [SerializeField] private Spikes _spikes;
    [SerializeField] private float _yPosition, _yStartPosition, _spikesActiveTime, _deactiveTime;
    [SerializeField] private DotweenSettings _dotweenSettings;
    private IEnumerator Start()
    {
        while (true)
        {
            _spikes.transform
                .DOLocalMoveY(_yPosition, _dotweenSettings.Duration)
                .SetEase(_dotweenSettings.AnimationType);

            yield return new WaitForSeconds(_spikesActiveTime);
        
            _spikes.transform
                .DOLocalMoveY(_yStartPosition, _dotweenSettings.Duration)
                .SetEase(_dotweenSettings.AnimationType);
            
            yield return new WaitForSeconds(_deactiveTime);
        }
    }
}

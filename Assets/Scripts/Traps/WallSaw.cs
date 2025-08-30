using System.Collections;
using DG.Tweening;
using UnityEngine;
public class WallSaw : MonoBehaviour
{
    [SerializeField] private Transform _saw;
    [SerializeField] private float _showTime, _deactiveTime, _xStartPosition, _xEndPosition;
    [SerializeField] private DotweenSettings _dotweenSettings;
    private IEnumerator Start()
    {
        while (true)
        {
            _saw.DOLocalMoveY(0, _dotweenSettings.Duration)
                .SetEase(_dotweenSettings.AnimationType);
            yield return new WaitForSeconds(1);
            
            _saw.DOLocalMoveZ(_xEndPosition, _dotweenSettings.Duration)
                .SetEase(_dotweenSettings.AnimationType);

            yield return new WaitForSeconds(3);
        
            _saw.DOLocalMoveZ(_xStartPosition, _dotweenSettings.Duration)
                .SetEase(_dotweenSettings.AnimationType);
        
            yield return new WaitForSeconds(2);
            _saw.DOLocalMoveY(-1, _dotweenSettings.Duration)
                .SetEase(_dotweenSettings.AnimationType);
            yield return new WaitForSeconds(2);
        }
    }
}

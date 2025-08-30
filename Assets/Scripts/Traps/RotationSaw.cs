using DG.Tweening;
using UnityEngine;
public class RotationSaw : MonoBehaviour
{
    [SerializeField] private Transform _rotationObject;
    [SerializeField] private DotweenSettings _dotweenSettings;
    private void Start()
    {
        _rotationObject
            .DOLocalRotate(new Vector3(359, 0, 0), _dotweenSettings.Duration, RotateMode.FastBeyond360)
            .SetEase(_dotweenSettings.AnimationType)
            .SetLoops(-1, LoopType.Incremental);
    }
}

using DG.Tweening;
using UnityEngine;
public class ChestSelector : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform[] _papers;
    [SerializeField] private Vector3[] _endPositions;

    [Header("Lid")]
    [SerializeField] private Transform _lid;
    [SerializeField] private float _lidXValue;

    [Header("Tween")]
    [SerializeField] private DotweenSettings _dotweenSettings;
    [SerializeField] private DotweenSettings _chestSettings;
    [SerializeField] private float _stagger = 0.08f;

    [Header("Arc shaping")]
    [SerializeField] private float _apexLeft   = 2.0f;
    [SerializeField] private float _apexCenter = 2.6f;
    [SerializeField] private float _apexRight  = 2.0f; 

    [SerializeField] private float _sideLeft   = -1.2f;
    [SerializeField] private float _sideCenter = 0.0f;
    [SerializeField] private float _sideRight  = 1.2f;
    private void Start()
    {
        _lid.DOLocalRotate(new Vector3(_lidXValue, 0, 0), _dotweenSettings.Duration)
            .SetEase(_dotweenSettings.AnimationType)
            .OnComplete(Launch);
    }

    private void Launch()
    {
        for (int i = 0; i < _papers.Length; i++)
        {
            var pb = _papers[i].GetComponent<PaperBehaviour>();
            var from = _papers[i].position;
            var to = _endPositions[i];
            
            float apex, side;
            if (i % 3 == 0) { apex = _apexLeft;   side = _sideLeft;   }
            else if (i % 3 == 1) { apex = _apexCenter; side = _sideCenter; }
            else { apex = _apexRight; side = _sideRight; }

            Vector3[] path = BuildArcPath(from, to, apex, side);
            pb.PlayFlyPath(path, _chestSettings.Duration, Ease.OutCubic, i * _stagger);
        }
    }
    
    private Vector3[] BuildArcPath(Vector3 start, Vector3 end, float apexHeight, float sideOffset)
    {
        Vector3 mid = (start + end) * 0.5f;
        Vector3 dir = (end - start).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, dir);
        
        Vector3 p1 = Vector3.Lerp(start, mid, 0.66f) + Vector3.up * apexHeight * 0.6f + right * sideOffset * 0.6f;
        Vector3 p2 = Vector3.Lerp(mid, end, 0.66f) + Vector3.up * apexHeight + right * sideOffset;

        return new[] { start, p1, p2, end };
    }
}

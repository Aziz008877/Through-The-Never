using DG.Tweening;
using UnityEngine;

public class PaperBehaviour : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform _model;
    [SerializeField] private Transform _textRoot;

    [Header("Idle float")]
    [SerializeField] private float _floatAmp = 0.03f;
    [SerializeField] private float _floatTime = 2.0f;

    [Header("Hover")]
    [SerializeField] private float _hoverScale = 1.06f;
    [SerializeField] private float _hoverIn = 0.14f;
    [SerializeField] private float _hoverOut = 0.14f;
    private Tween _floatMove;
    private Sequence _flight;
    private Vector3 _baseLocalPos;
    private Vector3 _baseLocalScale;
    private Quaternion _baseLocalRot;
    private bool _hovered;
    private void Awake()
    {
        _baseLocalPos = _model.localPosition;
        _baseLocalScale = _model.localScale;
        _baseLocalRot = _model.localRotation;
        _textRoot.gameObject.SetActive(false);
    }

    public void PlayFlyPath(Vector3[] worldPath, float duration, Ease ease, float delay)
    {
        KillAll();
        _model.position = worldPath[0];

        var col = GetComponent<Collider>();
        if (col) col.enabled = false;

        var move = _model.DOPath(worldPath, duration, PathType.CatmullRom, PathMode.Full3D)
                         .SetEase(ease)
                         .SetLookAt(0.001f);

        
        var bank = DOTween.Sequence()
            .Append(_model.DOLocalRotateQuaternion(Quaternion.Euler(5f, _model.localEulerAngles.y, -8f), duration * 0.35f).SetEase(Ease.InOutSine))
            .Append(_model.DOLocalRotateQuaternion(Quaternion.Euler(-3f, _model.localEulerAngles.y, 5f), duration * 0.35f).SetEase(Ease.InOutSine))
            .Append(_model.DOLocalRotateQuaternion(_baseLocalRot, duration * 0.30f).SetEase(Ease.InOutSine));

        _flight = DOTween.Sequence()
            .Append(move)
            .Join(bank)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                _baseLocalPos = _model.localPosition;
                _baseLocalRot = Quaternion.Euler(_baseLocalRot.eulerAngles.x, _model.localEulerAngles.y, _baseLocalRot.eulerAngles.z);

                if (col) col.enabled = true;
                if (_textRoot) _textRoot.gameObject.SetActive(true);
                StartIdleFloat();
            });
    }

    private void StartIdleFloat()
    {
        if (_hovered) return;
        _floatMove?.Kill();
        _model.localPosition = _baseLocalPos;
        
        _floatMove = _model.DOLocalMoveY(_baseLocalPos.y + _floatAmp, _floatTime * 0.5f)
                         .SetLoops(-1, LoopType.Yoyo)
                         .SetEase(Ease.InOutSine);
    }

    private void StopIdleFloat()
    {
        _floatMove?.Kill(); _floatMove = null;
        _model.DOLocalMoveY(_baseLocalPos.y, 0.12f);
        _model.DOLocalRotateQuaternion(Quaternion.Euler(_baseLocalRot.eulerAngles.x, 0f, _baseLocalRot.eulerAngles.z), 0.12f);
    }

    private void KillAll()
    {
        StopIdleFloat();
        _flight?.Kill(); _flight = null;
    }
    
    private void OnMouseEnter()
    {
        _hovered = true;
        StopIdleFloat();
        _model.DOLocalRotate(new Vector3(_model.localEulerAngles.x, 0f, _model.localEulerAngles.z), _hoverIn).SetEase(Ease.OutSine);
        _model.DOScale(_baseLocalScale * _hoverScale, _hoverIn).SetEase(Ease.OutSine);
    }

    private void OnMouseExit()
    {
        _hovered = false;
        _model.DOScale(_baseLocalScale, _hoverOut).SetEase(Ease.OutSine);
        StartIdleFloat();
    }
}

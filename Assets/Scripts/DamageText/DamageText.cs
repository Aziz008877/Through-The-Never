using DG.Tweening;
using UnityEngine;
using TMPro;
public class DamageText : MonoBehaviour
{
    [SerializeField] private Color _endColor;
    [SerializeField] private DotweenSettings _dotweenSettings;
    private TextMeshPro _text;
    private Camera _mainCamera;
    private void OnEnable()
    {
        _mainCamera = Camera.main;
    }
    public void Show(float amount, Vector3 position)
    {
        _text = GetComponent<TextMeshPro>();
        transform.position = position + Vector3.up * 1.5f;
        _text.text = $"{amount}";
        _text.alpha = 1f;
        
        Vector3 camPos = _mainCamera.transform.position;
        Vector3 lookDir = (transform.position - camPos).normalized;
        lookDir.y = 0f; 
        transform.forward = lookDir;

        gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();

        seq.Join(transform.DOMoveY(transform.position.y + 1.5f, _dotweenSettings.Duration).SetEase(_dotweenSettings.AnimationType));
        //seq.Join(_text.DOColor(_endColor, _dotweenSettings.Duration));

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
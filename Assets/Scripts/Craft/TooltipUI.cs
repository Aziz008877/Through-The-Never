using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    [Header("UI refs")]
    [SerializeField] private CanvasGroup _cg;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private RectTransform _rect;

    [Header("Settings")]
    [SerializeField] private Vector2 _screenOffset = new Vector2(16, -16);
    private static TooltipUI _inst;
    private void Awake() => _inst = this;
    private void OnDisable() => Hide();
    public static void Show(string text, Vector3 screenPos)
    {
        if (_inst == null) return;

        _inst._label.text = text;
        _inst._rect.pivot = new Vector2(0, 1);
        _inst._rect.position = screenPos + (Vector3)_inst._screenOffset;
        _inst._cg.alpha = 1;
    }

    public static void Hide()
    {
        if (_inst == null) return;
        _inst._cg.alpha = 0;
    }
}
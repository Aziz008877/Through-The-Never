using UnityEngine;
using TMPro;

public sealed class NemesisMarkUI : MonoBehaviour
{
    [SerializeField] private Canvas _worldCanvas;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private string _prefix = "N";

    public void SetLevel(int lvl)
    {
        if (_levelText) _levelText.text = $"{_prefix}{lvl}";
    }

    private void LateUpdate()
    {
        if (!_worldCanvas) return;
        var cam = Camera.main;
        if (!cam) return;
        transform.forward = cam.transform.forward;
    }
}
using UnityEngine;

public class EmissionRunes : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color _fireColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color _iceColor;
    [SerializeField] private Material _runesMaterial;
    private readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public void SetFireEmission()
    {
        SetEmissionColor(_fireColor);
    }

    public void SetIceEmission()
    {
        SetEmissionColor(_iceColor);
    }

    public void SetEmissionColor(Color color)
    {
        _runesMaterial.SetColor(EmissionColor, color);
    }
}
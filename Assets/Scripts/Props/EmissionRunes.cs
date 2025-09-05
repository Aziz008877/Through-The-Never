using UnityEngine;

public class EmissionRunes : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color _fireColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color _iceColor;
    [SerializeField] private Material _runesMaterial;
    private readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public void SetEmissionColor(MagicSchool school)
    {
        if (school == MagicSchool.Fire)
        {
            _runesMaterial.SetColor(EmissionColor, _fireColor);
        }
        else
        {
            _runesMaterial.SetColor(EmissionColor, _iceColor);
        }
        
    }
}
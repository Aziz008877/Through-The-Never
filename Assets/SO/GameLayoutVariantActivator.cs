using UnityEngine;

public class GameLayoutVariantActivator : MonoBehaviour
{
    [SerializeField] private LevelFlowAsset _flow;
    [SerializeField] private GameObject[] _variants;

    private void Awake()
    {
        foreach (var variant in _variants)
        {
            if (variant) variant.SetActive(false);
        }
    }

    private void Start()
    {
        int index = 0;

        if (_flow.TryGetCurrent(out var step))
        {
            index = (int)step.Variant; // 👈 приведение enum → int
        }

        if (index >= 0 && index < _variants.Length && _variants[index])
        {
            _variants[index].SetActive(true);
        }
        else if (_variants.Length > 0 && _variants[0]) // fallback на 0-й вариант
        {
            _variants[0].SetActive(true);
        }
    }
}
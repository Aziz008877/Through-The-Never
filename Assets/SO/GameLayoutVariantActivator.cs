using UnityEngine;

public class GameLayoutVariantActivator : MonoBehaviour
{
    [SerializeField] private LevelFlowAsset _flow;
    [SerializeField] private GameObject _variant1;
    [SerializeField] private GameObject _variant2;
    private void Awake()
    {
        if (_variant1) _variant1.SetActive(false);
        if (_variant2) _variant2.SetActive(false);
    }

    private void Start()
    {
        if (!_flow.TryGetCurrent(out var step))
        {
            if (_variant1) _variant1.SetActive(true);
            return;
        }

        bool first = (step.Variant == 0);
        if (_variant1) _variant1.SetActive(first);
        if (_variant2) _variant2.SetActive(!first);
    }
}
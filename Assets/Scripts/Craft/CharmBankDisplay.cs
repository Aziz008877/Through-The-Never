using System.Collections.Generic;
using UnityEngine;

public class CharmBankDisplay : MonoBehaviour
{
    [SerializeField] private CharmBank _charmBank;
    [SerializeField] private GameObject _counterPrefab;
    [SerializeField] private Transform _container;
    [SerializeField] private CraftingUI _craftingUI;

    private void Start()
    {
        _charmBank.OnCharmChanged += (_, _) => Rebuild();
        Rebuild();
    }

    public void Rebuild()
    {
        foreach (Transform child in _container)
            Destroy(child.gameObject);

        foreach (var pair in _charmBank.GetAll())
        {
            var obj = Instantiate(_counterPrefab, _container);
            var counter = obj.GetComponent<CharmCounterUI>();
            counter.Init(pair.Key, pair.Value, _craftingUI);
        }
    }
}
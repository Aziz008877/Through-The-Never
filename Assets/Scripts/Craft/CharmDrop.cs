using UnityEngine;

public class CharmDrop : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CharmSO _charmSO;
    [SerializeField] private ItemSO _charmItem;
    [SerializeField] private int _amount = 10;

    [Header("Refs")]
    [SerializeField] private CharmBank _charmBank;
    [SerializeField] private ItemInventory _itemInv;
    [SerializeField] private InventoryUI _invUI;
    [SerializeField] private CharmBankDisplay _displayUI;

    private void OnTriggerEnter(Collider other)
    {
        _charmBank.Add(_charmSO, _amount);
        _itemInv.Add(_charmItem, _amount);
        _displayUI.Rebuild();
        _invUI.Refresh();

        Destroy(gameObject);
    }
}
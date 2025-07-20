using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO _item;
    [SerializeField] private ItemInventory _itemInventory;
    [SerializeField] private InventoryUI _inventoryUI;

    [field: SerializeField] public Transform InteractionUI { get; set; }
    [field: SerializeField] public bool CanInteract { get; set; } = true;

    public void PerformAction(GameObject player)
    {
        if (!CanInteract) return;

        _itemInventory.Add(_item);
        _inventoryUI.AddItemToUI(_item);

        CanInteract = false;
        Destroy(gameObject);
    }
}
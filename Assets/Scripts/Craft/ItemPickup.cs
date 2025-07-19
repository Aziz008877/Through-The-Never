using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO _item;
    [field: SerializeField] public Transform InteractionUI { get; set; }
    [field: SerializeField] public bool CanInteract { get; set; } = true;
    public void PerformAction(GameObject player)
    {
        if (!CanInteract) return;

        /*var inv = player.GetComponentInChildren<ItemInventory>();
        if (inv == null) return;

        inv.Add(_item);
        CanInteract = false;
        Destroy(gameObject);            // предмет исчезает*/
    }
}

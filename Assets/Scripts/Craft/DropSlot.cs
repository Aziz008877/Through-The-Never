using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private SlotKind _kind;
    [SerializeField] private CraftingUI _craftUI;
    [SerializeField] private ItemInventory _inv;
    [SerializeField] private Image _icon;
    public void OnDrop(PointerEventData ev)
    {
        var drag = ev.pointerDrag.GetComponent<DragItem>();
        if (drag == null) return;

        bool ok = _kind switch
        {
            SlotKind.Remnant => drag.ItemData.Role == ItemRole.Remnant,
            SlotKind.Charm   => drag.ItemData.Role == ItemRole.Charm,
            _                => false
        };
        if (!ok) return;

        drag.MarkPlaced(transform);
        _inv.Remove(drag.ItemData, 1);

        _icon.sprite = drag.ItemData.Icon;
        _icon.enabled = true;
        _craftUI.HandleDrop(_kind, drag.ItemData);
    }
}

public enum SlotKind { Remnant, Charm }
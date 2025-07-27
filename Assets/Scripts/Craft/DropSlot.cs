using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private CraftingUI _craftingUI;

    public void OnDrop(PointerEventData eventData)
    {
        var dragItem = eventData.pointerDrag.GetComponent<DragItem>();
        if (dragItem == null) return;
        
        dragItem.MarkPlaced(transform);
        
        _icon.sprite   = dragItem.ItemData.Icon;
        _icon.enabled  = true;
        
        _craftingUI.SelectItem(dragItem.ItemData);
    }
}
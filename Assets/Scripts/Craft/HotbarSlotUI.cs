using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class HotbarSlotUI : MonoBehaviour, IDropHandler
{
    [SerializeField] private int _idx;
    [SerializeField] private ItemInventory _inv;
    [SerializeField] private Image _icon;
    private void Start () { _inv.OnChanged += Refresh; Refresh(); }
    private void OnDestroy() => _inv.OnChanged -= Refresh;
    public void Refresh()
    {
        var item = _inv.GetHotbarSlot(_idx);
        _icon.enabled = item;
        if (item) _icon.sprite = item.Icon;
    }

    public void OnDrop(PointerEventData ev)
    {
        if (ev.pointerDrag.TryGetComponent(out DragItem dragItem))
        {
            if (dragItem.ItemData.Role != ItemRole.Scroll) return;
            
            if (_inv.SetHotbarSlot(_idx, dragItem.ItemData))
            {
                _inv.Remove(dragItem.ItemData, 1);
                Destroy(dragItem.gameObject);
            }
        }
    }
}
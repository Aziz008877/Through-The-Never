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
        var drag = ev.pointerDrag?.GetComponent<DragItem>();
        if (drag == null) return;
        if (drag.ItemData.Role != ItemRole.Scroll) return;

        // 1) пробуем положить в хот-бар
        if (_inv.SetHotbarSlot(_idx, drag.ItemData))
        {
            // 2) успешно → вычитаем 1 из стэка
            _inv.Remove(drag.ItemData, 1);

            // 3) убираем перетянутую иконку
            Destroy(drag.gameObject);
        }
        // SetHotbarSlot уже вызывает OnChanged → InventoryUI сам обновится
    }
}
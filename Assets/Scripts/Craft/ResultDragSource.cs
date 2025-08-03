using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResultDragSource : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ItemInventory _inv;
    [SerializeField] private Image _icon;
    private CanvasGroup _cg; private RectTransform _rt;
    private ItemSO _item;
    public void Show(ItemSO item) { _item = item; _icon.sprite = item.Icon; _icon.enabled = true; }

    public void OnBeginDrag(PointerEventData ev)
    {
        if (_item == null) return;
        _cg ??= gameObject.AddComponent<CanvasGroup>();
        _rt ??= (RectTransform) transform;
        _cg.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData ev) => _rt.anchoredPosition += ev.delta;
    public void OnEndDrag(PointerEventData ev)
    {
        _cg.blocksRaycasts = true;
        _rt.localPosition  = Vector3.zero;
    }
}
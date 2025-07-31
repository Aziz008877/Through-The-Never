using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _itemIconImage;
    public ItemSO ItemData { get; private set; }
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Transform _originalParent;
    private bool _placed;
    public void Init(ItemSO item)
    {
        Debug.Log(item.DisplayName);
        ItemData = item;
        _itemIconImage.sprite = item.Icon;
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _placed = false;
        _originalParent = transform.parent;
        _canvasGroup.blocksRaycasts = false;
        transform.SetParent(_originalParent.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / transform.lossyScale;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        if (!_placed)
        {
            transform.SetParent(_originalParent);
            transform.localPosition = Vector3.zero;
        }
    }
    
    public void MarkPlaced(Transform newParent)
    {
        _placed = true;
        transform.SetParent(newParent);
        transform.localPosition = Vector3.zero;
    }
}
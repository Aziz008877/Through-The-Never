using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _itemIconImage;
    [SerializeField] private TMP_Text _stackText; 
    public ItemSO ItemData { get; private set; }
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Transform _originalParent;
    private bool _placed;
    public void Init(ItemSO item, int stackCount = 1)
    {
        //Debug.Log(item.DisplayName);
        ItemData = item;
        _itemIconImage.sprite = item.Icon;
        _stackText.text = stackCount > 1 ? stackCount.ToString() : "";
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 pos = ((RectTransform)transform).TransformPoint(GetComponent<RectTransform>().rect.center);
        TooltipUI.Show(ItemData.DisplayName, pos);
    }

    public void OnPointerExit(PointerEventData eventData)
        => TooltipUI.Hide();
    
    public void MarkPlaced(Transform newParent)
    {
        _placed = true;
        transform.SetParent(newParent);
        transform.localPosition = Vector3.zero;
    }
}
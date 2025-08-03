using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CraftCharmSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [Header("UI")]
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _countLabel;
    [SerializeField] private Button _plusBtn;
    [SerializeField] private Button _minusBtn;

    [Header("Logic refs")]
    [SerializeField] private ItemInventory _inv;
    [SerializeField] private CraftingUI _craftUI;

    [Header("Config")]
    [SerializeField] private int _step = 100;
    private CharmSO _charmType;
    private ItemSO _charmItem;
    private int _amount;
    private void Awake()
    {
        _plusBtn.onClick.AddListener(() => Change(+_step));
        _minusBtn.onClick.AddListener(() => Change(-_step));
        RefreshUI();
    }
    
    public void OnDrop(PointerEventData ev)
    {
        var drag = ev.pointerDrag.GetComponent<DragItem>();
        if (drag == null) return;
        if (drag.ItemData.Role != ItemRole.Charm) return;

        var charm = drag.ItemData.CharmRef;
        if (charm == null) return;
        
        Destroy(drag.gameObject); 
        _charmType = charm;
        _charmItem = drag.ItemData;
        _amount    = 0;
        RefreshUI();
        _craftUI.SetCharmAmount(_charmType, _amount);
    }
    
    private void Change(int delta)
    {
        if (_charmType == null) return;
        if (delta == 0) return;

        if (delta > 0)
        {
            int have = _inv.GetCount(_charmItem);
            int take = Mathf.Min(delta, have);
            if (take == 0) return;

            _inv.Remove(_charmItem, take);
            _amount += take;
        }
        else
        {
            int give = Mathf.Min(-delta, _amount);
            if (give == 0) return;

            _inv.Add(_charmItem, give);
            _amount -= give;
        }

        RefreshUI();
        _craftUI.SetCharmAmount(_charmType, _amount);
    }
    
    private void RefreshUI()
    {
        bool has = _charmType != null;
        _icon.enabled = has;
        _icon.sprite  = has ? _charmType.Icon : null;

        _countLabel.text = has ? _amount.ToString() : "";
        _plusBtn.interactable  = has && _inv.GetCount(_charmItem) > 0;
        _minusBtn.interactable = has && _amount > 0;
    }
    
    public void OnPointerClick(PointerEventData ev)
    {
        if (ev.button != PointerEventData.InputButton.Right) return;
        if (_charmType == null || _amount == 0) return;

        _inv.Add(_charmItem, _amount);
        _amount = 0; _charmType = null; _charmItem = null;

        RefreshUI();
        _craftUI.SetCharmAmount(null, 0);
    }
}

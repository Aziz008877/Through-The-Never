using UnityEngine;
using Zenject;
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private ItemInventory _inv;
    [SerializeField] private GameObject _dragPrefab;
    [SerializeField] private Transform _grid;
    [SerializeField] private UIPanel _panel;
    [Inject] private PlayerInput _inp;
    private bool _opened;
    private void Awake()
    {
        _inp.ChangeInventoryState += Toggle;
        _inv.OnChanged+= Refresh;
    }
    private void OnDestroy()
    {
        _inp.ChangeInventoryState -= Toggle;
        _inv.OnChanged -= Refresh;
    }
    private void Toggle()
    {
        _opened = !_opened;
        if (_opened) { _panel.ActivatePanel(); Refresh(); }
        else         { _panel.DeactivatePanel(); }
    }

    public void Refresh()
    {
        if (!_opened) return;

        foreach (Transform c in _grid) Destroy(c.gameObject);

        foreach (var st in _inv.Bag)
        {
            if (st.Count <= 0) continue;
            var go = Instantiate(_dragPrefab, _grid);
            go.GetComponent<DragItem>().Init(st.Item, st.Count);
        }
    }
}
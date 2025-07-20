using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _dragItemPrefab;
    [SerializeField] private Transform _container;

    public void AddItemToUI(ItemSO item)
    {
        var obj = Instantiate(_dragItemPrefab, _container);
        var dragItem = obj.GetComponent<DragItem>();
        dragItem.Init(item);
    }
}
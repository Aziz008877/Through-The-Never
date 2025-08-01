using System;
using UnityEngine;
using Zenject;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _dragItemPrefab;
    [SerializeField] private Transform _container;
    [SerializeField] private UIPanel _inventoryPanel;
    [Inject] private PlayerInput _playerInput;
    private bool _isOpened = false;
    private void Awake()
    {
        _playerInput.ChangeInventoryState += ChangeInventoryPanelState;
    }

    private void ChangeInventoryPanelState()
    {
        _isOpened = !_isOpened;

        if (_isOpened)
        {
            _inventoryPanel.ActivatePanel();
        }
        else
        {
            _inventoryPanel.DeactivatePanel();
        }
    }

    public void AddItemToUI(ItemSO item)
    {
        var obj = Instantiate(_dragItemPrefab, _container);
        var dragItem = obj.GetComponent<DragItem>();
        dragItem.Init(item);
    }

    private void OnDestroy()
    {
        _playerInput.ChangeInventoryState -= ChangeInventoryPanelState;
    }
}
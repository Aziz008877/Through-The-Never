using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class CraftingUI : MonoBehaviour
{
    [SerializeField] private CraftingHandler _handler;
    private readonly List<CharmCost> _charmInputs = new();
    private ItemSO _inputItem;
    public void HandleDrop(SlotKind kind, ItemSO item)
    {
        switch (kind)
        {
            case SlotKind.Remnant:
                _inputItem = item;
                break;
            case SlotKind.Charm:
                SetCharmAmount(item.GetComponent<CharmSO>(), 500);
                break;
        }
    }
    
    public void SetCharmAmount(CharmSO charm, int amount)
    {
        var i = _charmInputs.FindIndex(c => c.CharmType == charm);
        if (i >= 0)
        {
            var temp = _charmInputs[i];
            temp.Amount = amount;
            _charmInputs[i] = temp;
        }
        else
        {
            _charmInputs.Add(new CharmCost { CharmType = charm, Amount = amount });
        }
    }

    public void Craft()
    {
        if (_inputItem == null)
        {
            Debug.LogWarning("Нет предмета для крафта.");
            return;
        }

        bool success = _handler.TryCraft(_inputItem, _charmInputs);
        //Debug.Log(success ? "Успешный крафт" : "Ничего не найдено");
    }
}
using System.Collections.Generic;
using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] private CraftingHandler _handler;
    private ItemSO _inputItem;
    private readonly List<CharmCost> _charmInputs = new();

    public void SelectItem(ItemSO item)
    {
        _inputItem = item;
        Debug.Log("Выбран предмет: " + item.DisplayName);
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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharmCounterUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private TMP_Text _valueText;
    [SerializeField] private Button _plusButton;

    private int _currentValue;
    private CharmSO _charm;
    private CraftingUI _craftingUI;
    private int _max;

    public void Init(CharmSO charm, int max, CraftingUI ui)
    {
        _charm = charm;
        _max = max;
        _craftingUI = ui;

        _icon.sprite = charm.Icon;
        _label.text = charm.DisplayName;
        _valueText.text = "0";

        _plusButton.onClick.AddListener(AddOne);
    }

    private void AddOne()
    {
        if (_currentValue >= _max) return;
        _currentValue+=100;
        _valueText.text = _currentValue.ToString();
        _craftingUI.SetCharmAmount(_charm, _currentValue);
    }
}
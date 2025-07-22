using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharmCounterUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private TMP_Text _valueText;
    [SerializeField] private Button _plusButton;
    [SerializeField] private Button _minusButton;
    [SerializeField] private int _step = 100;
    private int _currentValue;
    private CharmSO _charm;
    private CraftingUI _craftingUI;
    private CharmBank _charmBank;
    private int _max;

    public void Init(CharmSO charm, int max, CraftingUI ui, CharmBank bank)
    {
        _charm = charm;
        _max = max;
        _craftingUI = ui;
        _charmBank = bank;
        _icon.sprite = charm.Icon;
        _label.text = charm.DisplayName;
        _valueText.text = "0";

        _plusButton.onClick.AddListener(AddOne);
        _minusButton.onClick.AddListener(RemoveOne);

        bank.OnCharmChanged += OnBankChanged;
    }

    private void AddOne()
    {
        if (_currentValue >= _max) return;
        _currentValue += _step;
        _valueText.text = _currentValue.ToString();
        _craftingUI.SetCharmAmount(_charm, _currentValue);
    }
    
    private void RemoveOne()
    {
        if (_currentValue == 0) return;

        _currentValue = Mathf.Max(0, _currentValue - _step);
        _valueText.text = _currentValue.ToString();
        _craftingUI.SetCharmAmount(_charm, _currentValue);
    }

    private void OnBankChanged(CharmSO charm, int newAmount)
    {
        if (charm != _charm) return;

        _max = newAmount;

        if (_currentValue > _max)
            RemoveOne();
    }
    
    private void OnDestroy()                   // снимаем подписку
    {
        if (_charmBank != null)
            _charmBank.OnCharmChanged -= OnBankChanged;
    }
}
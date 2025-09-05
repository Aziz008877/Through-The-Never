using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArtifactRowView : MonoBehaviour
{
    [SerializeField] private ArtifactId _id;
    [SerializeField] private Image[] _pips;
    [SerializeField] private Button _upgradeBtn;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private TMP_Text _coinsText;
    private void OnEnable()
    {
        if (MetaProgressionService.Instance != null)
            MetaProgressionService.Instance.OnChanged += Refresh;
        _upgradeBtn.onClick.AddListener(OnUpgradeClicked);
        Refresh();
    }

    private void OnDisable()
    {
        if (MetaProgressionService.Instance != null)
            MetaProgressionService.Instance.OnChanged -= Refresh;
        _upgradeBtn.onClick.RemoveListener(OnUpgradeClicked);
    }

    private void Refresh()
    {
        var meta = MetaProgressionService.Instance;
        if (!meta) return;

        int lvl = meta.GetLevel(_id);
        int max = meta.GetMaxLevel();

        for (int i = 0; i < _pips.Length; i++)
        {
            bool on = i < lvl;
            _pips[i].color = on ? Color.white : new Color(1,1,1,0.25f);
        }

        int cost = meta.GetUpgradeCost(_id);
        _priceText.text = (lvl >= max) ? "MAX" : cost.ToString();

        _coinsText.text = meta.Coins.ToString();

        _upgradeBtn.interactable = meta.CanUpgrade(_id);
    }

    private void OnUpgradeClicked()
    {
        var meta = MetaProgressionService.Instance;
        if (!meta) return;
        if (meta.Upgrade(_id))
            Refresh();
    }
}
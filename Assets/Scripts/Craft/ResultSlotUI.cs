using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResultSlotUI : MonoBehaviour
{
    [SerializeField] private Image _icon;

    public void Show(ItemSO item)
    {
        _icon.enabled = true;
        _icon.sprite = item.Icon;
        StartCoroutine(Hide());
    }

    private IEnumerator Hide()
    {
        yield return new WaitForSeconds(1f);
        _icon.enabled = false;
    }
}

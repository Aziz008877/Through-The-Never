using UnityEngine;

public class CharmDrop : MonoBehaviour
{
    [SerializeField] private CharmSO _charmSO;
    [SerializeField] private int _amount = 10;
    [SerializeField] private CharmBank _charmBank;
    [SerializeField] private CharmBankDisplay _displayUI; // UI для обновления

    private void OnTriggerEnter(Collider other)
    {
        _charmBank.Add(_charmSO, _amount);
        _displayUI.Rebuild(); // Обновить UI
        Destroy(gameObject);
    }
}
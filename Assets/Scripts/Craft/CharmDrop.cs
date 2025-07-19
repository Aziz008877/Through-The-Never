using UnityEngine;
public class CharmDrop : MonoBehaviour
{
    public CharmSO School;
    public int Amount = 10;

    private void Start()
    {
        /*var bank = FindObjectOfType<CharmBank>();
        if (bank != null) bank.Add(School, Amount);
        Destroy(gameObject);            // сразу удаляем*/
    }
}
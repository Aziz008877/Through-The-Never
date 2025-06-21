using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [field: SerializeField] public Transform InteractionUI { get; set; }
    public void PerformAction(GameObject player)
    {
        throw new System.NotImplementedException();
    }
}

using UnityEngine;
using UnityEngine.Events;

public class ArtifactShop : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent _onShopInteracted;
    public Transform InteractionUI { get; set; }
    public bool CanInteract { get; set; }
    public void PerformAction(GameObject player)
    {
        _onShopInteracted?.Invoke();
    }
}


using UnityEngine;

public interface IInteractable
{
    Transform InteractionUI { get; set; }
    bool CanInteract { get; set; }

    void PerformAction(GameObject player);
}

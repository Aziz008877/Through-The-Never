
using UnityEngine;

public interface IInteractable
{
    Transform InteractionUI { get; set; }

    void PerformAction(GameObject player);
}

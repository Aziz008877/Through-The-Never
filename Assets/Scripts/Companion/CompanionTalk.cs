using UnityEngine;

public class CompanionTalk : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCHandler _npcHandler;
    public Transform InteractionUI { get; set; }
    public bool CanInteract { get; set; }
    public void PerformAction(GameObject player)
    {
        _npcHandler.StartConversation();
    }
}

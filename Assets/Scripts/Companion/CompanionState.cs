using UnityEngine;
public class CompanionState : MonoBehaviour, IActorState
{
    bool _active = true;
    public void ChangePlayerState(bool active) => _active = active;
    public bool IsActive => _active;
}
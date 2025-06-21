using System.Threading.Tasks;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [SerializeField] private CurrentPlayerState _currentPlayerState;
    public CurrentPlayerState CurrentPlayerState => _currentPlayerState;

    public void ActivatePlayerState()
    {
        _currentPlayerState = CurrentPlayerState.CanControl;
    }
    
    public void DeactivatePlayerState()
    {
        _currentPlayerState = CurrentPlayerState.CannotControl;
    }

    public async void ChangePlayerState(bool state)
    {
        if (state)
        {
            await Task.Delay(2000);
            ActivatePlayerState();
        }
        else
        {
            DeactivatePlayerState();
        }
    }
}

public enum CurrentPlayerState
{
    CanControl,
    CannotControl
}

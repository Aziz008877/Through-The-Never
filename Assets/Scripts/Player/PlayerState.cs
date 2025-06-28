using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerState : MonoBehaviour
{
    [SerializeField] private CurrentPlayerState _currentPlayerState;
    public CurrentPlayerState CurrentPlayerState => _currentPlayerState;
    [Inject] private PlayerAnimator _playerAnimator;
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
            _playerAnimator.DeactivatePlayerMove();
        }
    }
}

public enum CurrentPlayerState
{
    CanControl,
    CannotControl
}

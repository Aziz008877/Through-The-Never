using System;
using DG.Tweening;
using UnityEngine;
using Zenject;
public class Chest : MonoBehaviour, IInteractable
{
    [field: SerializeField] public Transform InteractionUI { get; set; }
    [field: SerializeField] public bool CanInteract { get; set; }
    [SerializeField] private Transform _chestUp;
    [SerializeField] private DotweenSettings _dotweenSettings;
    [Inject] private PlayerState _playerState;
    public Action OnChestOpened;
    public void PerformAction(GameObject player)
    {
        if (CanInteract)
        {
            _playerState.ChangePlayerState(false);
            CanInteract = false;
            
            _chestUp
                .DOLocalRotate(new Vector3(-90, 0, 0), _dotweenSettings.Duration)
                .SetEase(_dotweenSettings.AnimationType)
                .OnComplete(delegate
                {
                    OnChestOpened?.Invoke();
                });
        }
    }
}

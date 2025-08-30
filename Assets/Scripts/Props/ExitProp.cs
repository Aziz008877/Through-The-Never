using DG.Tweening;
using UnityEngine;

public class ExitProp : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _movableExit;
    [SerializeField] private float _moveYPos;
    [SerializeField] private DotweenSettings _dotweenSettings;
    [field: SerializeField] public Transform InteractionUI { get; set; }
    public bool CanInteract { get; set; }
    public void PerformAction(GameObject player)
    {
        _movableExit
            .DOLocalMoveY(_moveYPos, _dotweenSettings.Duration)
            .SetEase(_dotweenSettings.AnimationType);
    }
}

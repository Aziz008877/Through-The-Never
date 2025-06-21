using System;
using DG.Tweening;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [field: SerializeField] public Transform InteractionUI { get; set; }
    [SerializeField] private Transform _chestUp;
    [SerializeField] private DotweenSettings _dotweenSettings;
    public event Action OnChestOpened;
    public void PerformAction(GameObject player)
    {
        _chestUp
            .DOLocalRotate(new Vector3(-90, 0, 0), _dotweenSettings.Duration)
            .SetEase(_dotweenSettings.AnimationType)
            .OnComplete(delegate
            {
                OnChestOpened?.Invoke();
            });
    }
}

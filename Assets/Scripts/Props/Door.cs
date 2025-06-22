using System;
using UnityEngine;
using Zenject;

public class Door : MonoBehaviour, IInteractable
{
    [field: SerializeField] public Transform InteractionUI { get; set; }
    [SerializeField] private Material _doorMaterial;
    [ColorUsage(true, true)]
    [SerializeField] private Color _inactiveColor, _activeColor;
    [SerializeField] private Chest _chest;
    [Inject] private FadeInOutEffect _fade;
    private bool _canPerform = false;
    private void Awake()
    {
        _chest.OnChestOpened += ReceiveChestOpened;
        _doorMaterial.SetColor("_EmissionColor", _inactiveColor);
    }

    private void ReceiveChestOpened()
    {
        _canPerform = true;
        _doorMaterial.SetColor("_EmissionColor", _activeColor);
    }

    public void PerformAction(GameObject player)
    {
        if (_canPerform)
        {
            _fade.Fade(1);
        }
    }

    private void OnDestroy()
    {
        _chest.OnChestOpened -= ReceiveChestOpened;
    }
}

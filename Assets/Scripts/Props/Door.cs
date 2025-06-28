using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class Door : MonoBehaviour, IInteractable
{
    [field: SerializeField] public Transform InteractionUI { get; set; }
    [SerializeField] private Material _doorMaterial;
    [ColorUsage(true, true)]
    [SerializeField] private Color _inactiveColor, _activeColor;
    [SerializeField] private Chest _chest;
    [Inject] private FadeInOutEffect _fade;
    [field: SerializeField] public bool CanInteract { get; set; }

    private void Awake()
    {
        _chest.OnChestOpened += ReceiveChestOpened;
        _doorMaterial.SetColor("_EmissionColor", _inactiveColor);
    }

    private void ReceiveChestOpened()
    {
        CanInteract = true;
        _doorMaterial.SetColor("_EmissionColor", _activeColor);
    }

    public async void PerformAction(GameObject player)
    {
        if (CanInteract)
        {
            CanInteract = false;
            _fade.Fade(1);
            await Task.Delay(1000);
            SceneManager.LoadScene("FightScene");
        }
    }

    private void OnDestroy()
    {
        _chest.OnChestOpened -= ReceiveChestOpened;
    }
}

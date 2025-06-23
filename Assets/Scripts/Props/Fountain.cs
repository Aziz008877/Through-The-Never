using System;
using System.Collections;
using UnityEngine;

public class Fountain : MonoBehaviour, IInteractable
{
    [SerializeField] private float _healValue = 100;
    [field: SerializeField] public Transform InteractionUI { get; set; }
    [field: SerializeField] public bool CanInteract { get; set; }
    [SerializeField] private AudioSource _healSFX;
    public Action<bool> OnPlayerHealing;
    public void PerformAction(GameObject player)
    {
        if (player.TryGetComponent(out PlayerHP playerHp) && CanInteract)
        {
            CanInteract = false;
            playerHp.ReceiveHP(_healValue);
            StartCoroutine(Healing());
        }
    }

    private IEnumerator Healing()
    {
        _healSFX.Play();
        OnPlayerHealing?.Invoke(true);
        yield return new WaitForSeconds(3);
        OnPlayerHealing?.Invoke(false);
    }
}

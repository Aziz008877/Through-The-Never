using System;
using UnityEngine;
public class SchoolStatue : MonoBehaviour, IInteractable
{
    [SerializeField] private MagicSchool _school;
    [SerializeField] private SkillSelectionSaver _saver;
    [SerializeField] private ParticleSystem _chooseVFX;
    [field: SerializeField] public Transform InteractionUI { get; set; }
    [field: SerializeField] public bool CanInteract { get; set; }
    public Action<MagicSchool> OnMagicSchoolSelected;
    private void Awake()
    {
        InteractionUI.gameObject.SetActive(false);
    }

    public void PerformAction(GameObject player)
    {
        if (!_saver.TryChooseSchool(_school)) return;

        OnMagicSchoolSelected?.Invoke(_school);
        _chooseVFX.Play();
        InteractionUI.gameObject.SetActive(false);
        GetComponent<Collider>().enabled = false;
        Debug.Log($"School {_school} chosen");
    }
}
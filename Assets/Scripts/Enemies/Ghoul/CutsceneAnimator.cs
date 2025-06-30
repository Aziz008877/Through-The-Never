using System.Collections;
using UnityEngine;
public class EnemyResurrector : MonoBehaviour
{
    [SerializeField] private GameObject[] _allCutsceneObjects, _allInGameObjects;
    [SerializeField] private Animator _animator;
    [SerializeField] private string _deathStateName = "Death";
    [SerializeField] private string _standUpTrigger = "StandUp";
    [SerializeField] private CardHandler _cardHandler;
    [SerializeField] private FadeInOutEffect _fadeInOutEffect;
    private void Awake()
    {
        _cardHandler.OnCardSelected += CardSelected;
    }

    private void Start()
    {
        _animator.Play(_deathStateName, 0, 1f);
        _animator.Update(0f);
        _animator.speed = 0f;
    }
    
    private void Resurrect()
    {
        _animator.speed = 1f;
        _animator.SetTrigger(_standUpTrigger);
    }

    private void CardSelected()
    {
        StartCoroutine(ResurrectCoroutine());
    }

    private IEnumerator ResurrectCoroutine()
    {
        _fadeInOutEffect.FadeInOut(2);
        
        yield return new WaitForSeconds(1.5f);
        
        foreach (var cutsceneObject in _allCutsceneObjects)
        {
            cutsceneObject.SetActive(true);
        }
        /*_animator.Play(_deathStateName, 0, 1f);
        _animator.Update(0f);
        _animator.speed = 0f;*/
        
        foreach (var defaultObject in _allInGameObjects)
        {
            defaultObject.SetActive(false);
        }
        
        yield return new WaitForSeconds(2f);

        Resurrect();
        
        yield return new WaitForSeconds(2f);
        
        _fadeInOutEffect.FadeInOut(2);
        
        yield return new WaitForSeconds(1.5f);
        
        foreach (var cutsceneObject in _allCutsceneObjects)
        {
            cutsceneObject.SetActive(false);
        }
        
        foreach (var defaultObject in _allInGameObjects)
        {
            defaultObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        _cardHandler.OnCardSelected -= CardSelected;
    }
}
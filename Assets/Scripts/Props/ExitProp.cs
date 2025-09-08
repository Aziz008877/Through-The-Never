using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ExitProp : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _movableExit;
    [SerializeField] private float _moveYPos;
    [SerializeField] private DotweenSettings _dotweenSettings;
    [SerializeField] private LevelFlowRunner _flowRunner;
    [SerializeField] private UIFade _uiFade;
    public Transform InteractionUI { get; set; }
    [field: SerializeField] public bool CanInteract { get; set; }

    public void PerformAction(GameObject player)
    {
        if (!CanInteract) return;
        _uiFade.Fade(1);
        StartCoroutine(PerformWithDelay());
    }

    private IEnumerator PerformWithDelay()
    {
        yield return new WaitForSeconds(_uiFade.DotweenSettings.Duration + 0.5f);
        _movableExit
            .DOLocalMoveY(_moveYPos, _dotweenSettings.Duration)
            .SetEase(_dotweenSettings.AnimationType)
            .OnComplete(() =>
            {
                if (_flowRunner) _flowRunner.GoNext();
                else Debug.LogWarning("[ExitProp] LevelFlowRunner не назначен.");
            });
    }
}
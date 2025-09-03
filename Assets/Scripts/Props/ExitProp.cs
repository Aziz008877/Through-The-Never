using DG.Tweening;
using UnityEngine;

public class ExitProp : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _movableExit;
    [SerializeField] private float _moveYPos;
    [SerializeField] private DotweenSettings _dotweenSettings;
    [SerializeField] private LevelFlowRunner _flowRunner; // <-- укажи из сцены boot (объект DontDestroyOnLoad)
    [field: SerializeField] public Transform InteractionUI { get; set; }
    public bool CanInteract { get; set; }

    public void PerformAction(GameObject player)
    {
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
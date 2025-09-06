using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ChestSelector : MonoBehaviour, IInteractable
{
    [Header("Targets")] 
    [SerializeField] private Animator _chestAnimator;
    [SerializeField] private SkillPaper[] _papers;
    [SerializeField] private Vector3[] _endPositions;
    [SerializeField] private float _waitTime;

    [Header("Tween")]
    [SerializeField] private DotweenSettings _chestSettings;
    [SerializeField] private float _stagger = 0.08f;

    [Header("Arc shaping")]
    [SerializeField] private float _apexLeft   = 2.0f;
    [SerializeField] private float _apexCenter = 2.6f;
    [SerializeField] private float _apexRight  = 2.0f; 
    [SerializeField] private float _sideLeft   = -1.2f;
    [SerializeField] private float _sideCenter = 0.0f;
    [SerializeField] private float _sideRight  = 1.2f;

    [Header("Offers")]
    [SerializeField] private ChestOfferDirector _director;  
    [SerializeField] private SkillSelectionSaver _saver; 
    [SerializeField] private MagicSchool _fallbackSchoolIfNone;
    [SerializeField] private UnityEvent _onChestOpened, _onChestClosed;
    private List<SkillDefinition> _currentOffer;
    private bool _choiceLocked;

    private IEnumerator OpenChest()
    {
        _onChestOpened?.Invoke();
        _chestAnimator.SetTrigger("OpenChest");
        yield return new WaitForSeconds(_waitTime);

        if (_director == null)
        {
            Debug.LogError("[ChestSelector] No ChestOfferDirector set");
            yield break;
        }
        var fb = (_saver != null) ? _saver.School : _fallbackSchoolIfNone;
        _director.EnsureInitialized(fb);
        
        if (!_director.TryGetNextOffer(out var stage, out var offer))
        {
            Debug.Log("[ChestSelector] No offers left.");
            yield break;
        }

        _currentOffer = offer;
        
        for (int i = 0; i < _papers.Length; i++)
        {
            var paper = _papers[i];
            paper.Clicked -= OnPaperClicked;
            paper.Clicked += OnPaperClicked;

            SkillDefinition def = (i < offer.Count) ? offer[i] : null;
            paper.Init(def);
        }

        Launch();
    }

    private void Launch()
    {
        for (int i = 0; i < _papers.Length; i++)
        {
            var paperTr = _papers[i].transform;
            var pb = _papers[i].GetComponent<PaperBehaviour>();
            if (!paperTr.gameObject.activeSelf) continue;

            var from = paperTr.position;
            var to = _endPositions[i];

            float apex, side;
            if (i % 3 == 0) { apex = _apexLeft;   side = _sideLeft;   }
            else if (i % 3 == 1) { apex = _apexCenter; side = _sideCenter; }
            else { apex = _apexRight; side = _sideRight; }

            Vector3[] path = BuildArcPath(from, to, apex, side);
            pb.PlayFlyPath(path, _chestSettings.Duration, Ease.OutCubic, i * _stagger);
        }
    }

    private Vector3[] BuildArcPath(Vector3 start, Vector3 end, float apexHeight, float sideOffset)
    {
        Vector3 mid = (start + end) * 0.5f;
        Vector3 dir = (end - start).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, dir);

        Vector3 p1 = Vector3.Lerp(start, mid, 0.66f) + Vector3.up * apexHeight * 0.6f + right * sideOffset * 0.6f;
        Vector3 p2 = Vector3.Lerp(mid, end, 0.66f)   + Vector3.up * apexHeight         + right * sideOffset;

        return new[] { start, p1, p2, end };
    }

    private void OnPaperClicked(SkillPaper paper)
    {
        if (_choiceLocked) return;
        if (paper == null || paper.Definition == null) return;

        _choiceLocked = true;

        _director.AcceptChoice(paper.Definition);

        for (int i = 0; i < _papers.Length; i++)
        {
            if (_papers[i] != paper)
                _papers[i].gameObject.SetActive(false);
        }
        _onChestClosed?.Invoke();
    }

    public Transform InteractionUI { get; set; }
    public bool CanInteract { get; set; }
    public void PerformAction(GameObject player)
    {
        StartCoroutine(OpenChest());
    }
}

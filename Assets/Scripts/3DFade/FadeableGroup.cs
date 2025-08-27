using System.Collections.Generic;
using UnityEngine;

public class FadeableGroup : MonoBehaviour, IFadeable
{
    [Header("What to fade")]
    [SerializeField] private bool _autoCollectChildren = true;
    [SerializeField] private Renderer[] _renderers;

    [Header("Fade params")]
    [SerializeField, Range(0f, 1f)] private float _fadeTo = 0.1f;
    [SerializeField] private float _fadeSpeed = 5f;
    [SerializeField] private float _unfadeSpeed = 6f;

    [Header("Debug")]
    [SerializeField] private bool _debugLogs = false;
    private static readonly int FadeProp = Shader.PropertyToID("_Fade");
    private readonly Dictionary<Renderer, float> _current = new();
    private readonly HashSet<Renderer> _warnedNoFade = new();
    private MaterialPropertyBlock _mpb;
    private bool _wantFade;
    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        if (_autoCollectChildren || _renderers == null || _renderers.Length == 0)
            _renderers = GetComponentsInChildren<Renderer>(true);
        
        foreach (var r in _renderers)
            if (r && !_current.ContainsKey(r)) _current[r] = 1f;
    }

    private void OnValidate()
    {
        if (_autoCollectChildren)
            _renderers = GetComponentsInChildren<Renderer>(true);
    }

    private void Update()
    {
        if (_renderers == null || _renderers.Length == 0) return;

        float dt = Time.deltaTime;
        foreach (var r in _renderers)
        {
            if (!r) continue;
            
            if (!AnyMaterialHasFade(r))
            {
                if (_warnedNoFade.Add(r) && _debugLogs)
                    Debug.LogWarning($"[FadeableGroup:{name}] '{r.name}' шейдер без _Fade. Пропуcк.");
                continue;
            }

            float cur = _current.TryGetValue(r, out var v) ? v : 1f;
            float target = _wantFade ? _fadeTo : 1f;
            float speed = _wantFade ? _fadeSpeed : _unfadeSpeed;
            float next = Mathf.MoveTowards(cur, target, speed * dt);
            if (!Mathf.Approximately(cur, next))
            {
                _current[r] = next;
                ApplyFade(r, next);
            }
        }
    }

    public void FadeAll()
    {
        _wantFade = true;
    }

    public void UnfadeAll()
    {
        _wantFade = false;
    }

    private void ApplyFade(Renderer r, float fade)
    {
        var mats = r.sharedMaterials;
        for (int i = 0; i < mats.Length; i++)
        {
            r.GetPropertyBlock(_mpb, i);
            _mpb.SetFloat(FadeProp, fade);
            r.SetPropertyBlock(_mpb, i);
        }
    }

    private static bool AnyMaterialHasFade(Renderer r)
    {
        var mats = r.sharedMaterials;
        for (int i = 0; i < mats.Length; i++)
        {
            var m = mats[i];
            if (m != null && m.HasProperty(FadeProp))
                return true;
        }
        return false;
    }
}

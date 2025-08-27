using UnityEngine;
using System.Collections.Generic;
public sealed class IsoFadeOccluders : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform _player;
    [SerializeField] private Camera _cam;

    [Header("Ray Settings")]
    [SerializeField] private float _rayRadius = 0.15f;

    private readonly HashSet<IFadeable> _hitThisFrame = new();
    private readonly HashSet<IFadeable> _activeFaded = new();

    private void Awake()
    {
        if (!_cam) _cam = Camera.main;
    }

    private void Update()
    {
        _hitThisFrame.Clear();

        Vector3 a = _cam.transform.position;
        Vector3 b = _player.position;
        Vector3 dir = (b - a).normalized;
        float dist = Vector3.Distance(a, b);

        var hits = Physics.SphereCastAll(a, _rayRadius, dir, dist);

        foreach (var h in hits)
        {
            if (h.collider.TryGetComponent(out IFadeable fadeable))
            {
                _hitThisFrame.Add(fadeable);
                if (_activeFaded.Add(fadeable))
                {
                    fadeable.FadeAll();
                }
            }
        }
        
        var toRestore = ListPool<IFadeable>.Get();
        toRestore.AddRange(_activeFaded);

        foreach (var f in toRestore)
        {
            if (_hitThisFrame.Contains(f)) continue;
            _activeFaded.Remove(f);
            f.UnfadeAll();
        }
        ListPool<IFadeable>.Release(toRestore);
    }
    
    static class ListPool<T>
    {
        static readonly Stack<List<T>> _pool = new();
        public static List<T> Get() => _pool.Count > 0 ? _pool.Pop() : new List<T>(32);
        public static void Release(List<T> list) { list.Clear(); _pool.Push(list); }
    }
}

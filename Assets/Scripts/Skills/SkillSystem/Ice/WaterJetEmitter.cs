using UnityEngine;

public class WaterJetEmitter : MonoBehaviour
{
    [SerializeField] private ParticleSystem _jetVfx;
    [SerializeField] private Transform _scaleRoot;

    private Transform _pivot;

    public void Bind(Transform pivot, float range)
    {
        _pivot = pivot;
        if (_scaleRoot) _scaleRoot.localScale = new Vector3(_scaleRoot.localScale.x, _scaleRoot.localScale.y, range);
        if (_jetVfx) _jetVfx.Play();
        Update();
    }

    private void Update()
    {
        if (_pivot == null) return;
        transform.position = _pivot.position;
        transform.rotation = _pivot.rotation;
    }
}
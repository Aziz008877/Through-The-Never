using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;
    private void LateUpdate()
    {
        if (_target == null) return;
        transform.position = _target.position + _offset;
    }
}
using UnityEngine;

public class IceCage : MonoBehaviour
{
    [SerializeField] private float _freezeTime;
    public float FreezeTime => _freezeTime;

    public void ExplodeCage()
    {
        Destroy(gameObject);
    }
}

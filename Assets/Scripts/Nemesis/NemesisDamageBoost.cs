using UnityEngine;

public sealed class NemesisDamageBoost : MonoBehaviour
{
    public float Mult { get; private set; } = 1f;
    public void Init(float mult) => Mult = Mathf.Max(1f, mult);
}
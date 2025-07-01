using System.Collections.Generic;
using UnityEngine;

public class DamageTextPool : MonoBehaviour
{
    [SerializeField] private DamageText _damageTextPrefab;
    [SerializeField] private int _poolSize = 20;

    private Queue<DamageText> _pool = new();

    private void Awake()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            DamageText instance = Instantiate(_damageTextPrefab, transform);
            instance.gameObject.SetActive(false);
            _pool.Enqueue(instance);
        }
    }

    public void ShowDamage(float amount, Vector3 position)
    {
        DamageText dt = _pool.Dequeue();
        dt.Show(amount, position);
        _pool.Enqueue(dt);
    }
}
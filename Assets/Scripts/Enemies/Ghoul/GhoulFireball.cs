using System;
using UnityEngine;

public class GhoulFireball : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private float _speed;
    
    private void Update()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerHP playerHp))
        {
            playerHp.ReceiveDamage(_damage);
        }
    }
}

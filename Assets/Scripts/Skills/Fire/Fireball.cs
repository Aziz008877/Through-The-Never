using System;
using System.Threading.Tasks;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private ParticleSystem _castParticles;
    [SerializeField] private ParticleSystem _fireballParticles;
    [SerializeField] private ParticleSystem _fireballHit;
    [SerializeField] private float _speed;
    private float _damage;
    public void Init(float damage, float duration)
    {
        _damage = damage;
        _castParticles.Play();
        Invoke(nameof(DestroyFireball), duration);
    }
    private void Update()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    private async void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        DestroyFireball();
    }

    private async void DestroyFireball()
    {
        _fireballHit.Play();
        _fireballParticles.gameObject.SetActive(false);
        _speed = 0;

        await Task.Delay(1500);

        if (this != null && gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}

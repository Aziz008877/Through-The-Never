using System;
using UnityEngine;
public class StunDebuff : MonoBehaviour
{
    private BaseEnemyAnimation _baseEnemyAnimation;
    private BaseEnemyAttack _baseEnemyAttack;
    private BaseEnemyMove _baseEnemyMove;
    private float _stunTimeLeft;
    private bool _isStunned;
    private void Start()
    {
        _baseEnemyAnimation = GetComponent<BaseEnemyAnimation>();
        _baseEnemyAttack = GetComponent<BaseEnemyAttack>();
        _baseEnemyMove = GetComponent<BaseEnemyMove>();
    }

    public void ApplyStun(float duration)
    {
        Debug.Log("STUNNED");
        _stunTimeLeft = duration;
        _isStunned = true;
        _baseEnemyAnimation.Stun(_isStunned);
        _baseEnemyMove.SetMoveState(false);
    }
    
    private void Update()
    {
        if (!_isStunned) return;

        _stunTimeLeft -= Time.deltaTime;
        if (_stunTimeLeft <= 0f)
        {
            EndStun();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IceCage cage))
        {
            ApplyStun(cage.FreezeTime);
            cage.ExplodeCage();
        }
    }

    private void EndStun()
    {
        _isStunned = false;
        _baseEnemyAnimation.Stun(_isStunned);
        _baseEnemyMove.SetMoveState(true);
    }
}

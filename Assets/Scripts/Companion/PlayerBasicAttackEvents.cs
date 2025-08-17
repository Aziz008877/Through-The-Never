using System;
using UnityEngine;

public static class PlayerBasicAttackEvents
{
    public static Action<Vector3> OnBasicAttack;
    public static void Fire(Vector3 point) => OnBasicAttack?.Invoke(point);
}
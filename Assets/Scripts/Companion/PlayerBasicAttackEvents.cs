using System;
using UnityEngine;

public static class PlayerBasicAttackEvents
{
    /// <param name="Vector3">Мировые координаты точки, куда был выстрел</param>
    public static Action<Vector3> OnBasicAttack;
    public static void Fire(Vector3 point) => OnBasicAttack?.Invoke(point);
}
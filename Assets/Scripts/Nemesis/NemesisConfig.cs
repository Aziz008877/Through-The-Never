using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NemesisConfig", menuName = "Nemesis/NemesisConfig")]
public class NemesisConfig : ScriptableObject
{
    [Header("Progression")]
    [SerializeField] private int _maxLevel = 10;
    [SerializeField] private float _hpPerLevel = 0.15f;
    [SerializeField] private float _damagePerLevel = 0.10f;
    [SerializeField] private float _moveSpeedPerLevel = 0.06f;
    public int MaxLevel => _maxLevel;
    public float HpPerLevel => _hpPerLevel;
    public float DamagePerLevel => _damagePerLevel;
    public float MoveSpeedPerLevel => _moveSpeedPerLevel;
}

[Serializable]
public struct NemesisRecord
{
    public string NpcId;
    public int Level;
    public long LastUpdateUnix;
}


using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NemesisConfig", menuName = "Nemesis/NemesisConfig")]
public class NemesisConfig : ScriptableObject
{
    [Header("Progression")]
    [SerializeField] private int _maxLevel = 10;
    [SerializeField] private float _hpPerLevel = 0.15f;
    [SerializeField] private float _damagePerLevel = 0.10f;

    [Header("UI")]
    [SerializeField] private NemesisMarkUI _markPrefab;
    [SerializeField] private Vector3 _markOffset = new Vector3(0, 2.2f, 0);

    public int MaxLevel => _maxLevel;
    public float HpPerLevel => _hpPerLevel;
    public float DamagePerLevel => _damagePerLevel;
    public NemesisMarkUI MarkPrefab => _markPrefab;
    public Vector3 MarkOffset => _markOffset;
}

[Serializable]
public struct NemesisRecord
{
    public string NpcId;
    public int Level;
    public long LastUpdateUnix;
}
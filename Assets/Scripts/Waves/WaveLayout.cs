using UnityEngine;

[CreateAssetMenu(menuName = "Game/Spawning/Wave Layout", fileName = "WaveLayout")]
public class WaveLayout : ScriptableObject
{
    [System.Serializable] public struct WaveEntry { public EnemyKind Kind; public int Tier; public int Count; }
    [System.Serializable] public struct Wave { public WaveEntry[] Entries; }

    [Header("Список волн для этого Layout")]
    public Wave[] Waves;
}
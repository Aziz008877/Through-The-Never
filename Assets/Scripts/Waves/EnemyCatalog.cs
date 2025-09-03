using UnityEngine;

public enum EnemyKind { Ranged, Melee }

[CreateAssetMenu(menuName = "Game/Spawning/Enemy Catalog", fileName = "EnemyCatalog")]
public class EnemyCatalog : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        public EnemyKind Kind;
        public BaseEnemyHP Prefab;
    }

    [SerializeField] private Entry[] _entries;

    public bool TryGet(EnemyKind kind, out BaseEnemyHP prefab)
    {
        for (int i = 0; i < _entries.Length; i++)
        {
            if (_entries[i].Kind == kind && _entries[i].Prefab)
            {
                prefab = _entries[i].Prefab;
                return true;
            }
        }

        prefab = null;
        return false;
    }
}
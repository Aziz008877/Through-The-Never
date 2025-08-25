using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class FileNemesisStorage : INemesisStorage
{
    private readonly string _path;

    [Serializable]
    private class Payload
    {
        public List<NemesisRecord> Items = new();
    }

    public FileNemesisStorage()
    {
        _path = Path.Combine(Application.persistentDataPath, "nemesis.json");
    }

    public Dictionary<string, int> LoadAll()
    {
        try
        {
            if (!File.Exists(_path)) return new Dictionary<string, int>();
            var json = File.ReadAllText(_path);
            var data = JsonUtility.FromJson<Payload>(json);
            var dict = new Dictionary<string, int>(StringComparer.Ordinal);
            if (data?.Items != null)
                foreach (var r in data.Items)
                    dict[r.NpcId] = Mathf.Max(0, r.Level);
            return dict;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Nemesis load failed: {e.Message}");
            return new Dictionary<string, int>();
        }
    }

    public void SaveAll(Dictionary<string, int> levels)
    {
        try
        {
            var p = new Payload();
            foreach (var kv in levels)
                p.Items.Add(new NemesisRecord {
                    NpcId = kv.Key,
                    Level = kv.Value,
                    LastUpdateUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });

            var json = JsonUtility.ToJson(p, prettyPrint: true);
            File.WriteAllText(_path, json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Nemesis save failed: {e.Message}");
        }
    }
}
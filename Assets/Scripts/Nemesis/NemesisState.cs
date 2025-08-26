using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class NemesisState
{
    public List<string> Ids = new();
    public List<int> Levels = new();
    public string LastKillerId;
}

public static class NemesisStorage
{
    private static string Path => System.IO.Path.Combine(Application.persistentDataPath, "nemesis_state.json");

    public static NemesisState Load()
    {
        try
        {
            if (!File.Exists(Path)) return new NemesisState();
            var json = File.ReadAllText(Path);
            var state = JsonUtility.FromJson<NemesisState>(json) ?? new NemesisState();
            return state;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"NemesisStorage.Load: {e.Message}");
            return new NemesisState();
        }
    }

    public static void Save(NemesisState state)
    {
        try
        {
            var json = JsonUtility.ToJson(state, prettyPrint: true);
            File.WriteAllText(Path, json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"NemesisStorage.Save: {e.Message}");
        }
    }
}
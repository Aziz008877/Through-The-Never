using System;
using System.IO;
using UnityEngine;

[Serializable]
public class NemesisState
{
    public bool Active;
    public string Kind;
    public int BaseTier;
    public int Level;

    public override string ToString()
        => $"Active={Active}, Kind={Kind}, BaseTier={BaseTier}, Level={Level}";
}


public static class NemesisStorage
{
    private static string FilePath => Path.Combine(Application.persistentDataPath, "nemesis_state.json");

    public static NemesisState Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return new NemesisState();
            var json = File.ReadAllText(FilePath);
            var s = JsonUtility.FromJson<NemesisState>(json) ?? new NemesisState();
            return s;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Nemesis] Load error: {e.Message}");
            return new NemesisState();
        }
    }

    public static void Save(NemesisState s)
    {
        try
        {
            var json = JsonUtility.ToJson(s, prettyPrint: true);
            File.WriteAllText(FilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Nemesis] Save error: {e.Message}");
        }
    }
}
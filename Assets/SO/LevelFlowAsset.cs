using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Flow", fileName = "LevelFlowAsset")]
public class LevelFlowAsset : ScriptableObject
{
    public enum LayoutVariant { First = 1, Second = 2 }

    [System.Serializable]
    public struct Step
    {
        public string Scene;
        [Tooltip("1 = Layout 1, 2 = Layout 2")]
        public LayoutVariant Variant;

        [Tooltip("Набор волн для ЭТОГО захода на сцену/лейаут")]
        public WaveLayout WaveLayout; // <-- НОВОЕ поле
    }

    [Header("Шаги потока")]
    public Step[] Steps;

    [Header("Сейв прогресса")]
    public bool SaveProgress = false;
    public string PlayerPrefsKey = "level_flow_index";

    [System.NonSerialized] public int Index;

    public void LoadSavedOrReset()
    {
        Index = SaveProgress ? PlayerPrefs.GetInt(PlayerPrefsKey, 0) : 0;
        Index = Mathf.Clamp(Index, 0, Mathf.Max(0, Steps.Length - 1));
    }

    public void Save()
    {
        if (SaveProgress) PlayerPrefs.SetInt(PlayerPrefsKey, Index);
    }

    public bool TryGetCurrent(out Step step)
    {
        if (Steps == null || Steps.Length == 0) { step = default; return false; }
        Index = Mathf.Clamp(Index, 0, Steps.Length - 1);
        step = Steps[Index];
        return true;
    }

    public bool TryAdvance(out Step next)
    {
        Index++;
        Save();
        if (Index >= (Steps?.Length ?? 0)) { next = default; return false; }
        next = Steps[Index];
        return true;
    }
}
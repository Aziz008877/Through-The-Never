using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Flow", fileName = "LevelFlowAsset")]
public class LevelFlowAsset : ScriptableObject
{
    [System.Serializable]
    public struct Step
    {
        public string Scene;
        public int Variant;
    }

    [Header("Последовательность шагов")]
    public Step[] Steps;

    [Header("Сохранение прогресса")]
    public bool SaveProgress = false;
    public string PlayerPrefsKey = "level_flow_index";

    [System.NonSerialized] public int Index; // runtime

    public void ResetToStart()
    {
        Index = 0;
    }

    public void LoadSavedOrReset()
    {
        Index = SaveProgress ? PlayerPrefs.GetInt(PlayerPrefsKey, 0) : 0;
        Index = Mathf.Clamp(Index, 0, Mathf.Max(0, Steps.Length - 1));
    }

    public void Save()
    {
        if (SaveProgress)
            PlayerPrefs.SetInt(PlayerPrefsKey, Index);
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
        if (Index >= (Steps?.Length ?? 0))
        {
            next = default;
            return false; // поток закончился
        }
        next = Steps[Index];
        return true;
    }
}
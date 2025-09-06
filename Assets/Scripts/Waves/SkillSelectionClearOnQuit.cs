using UnityEngine;

public sealed class SkillSelectionClearOnQuit : MonoBehaviour
{
    [SerializeField] private SkillSelectionSaver _saver;

    private void OnApplicationQuit()
    {
        if (_saver) _saver.Clear();
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
    {
        if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode && _saver)
        {
            _saver.Clear();
            UnityEditor.EditorUtility.SetDirty(_saver);
        }
    }
#endif
}
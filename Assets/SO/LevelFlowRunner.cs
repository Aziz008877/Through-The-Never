using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFlowRunner : MonoBehaviour
{
    [SerializeField] private LevelFlowAsset _flow;

    private void Start()
    {
        _flow.TryGetCurrent(out var step);
    }

    public void GoNext()
    {
        if (_flow.TryAdvance(out var step))
            SceneManager.LoadSceneAsync(step.Scene);
    }
}
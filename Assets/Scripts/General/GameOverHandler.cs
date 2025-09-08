using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] private SkillSelectionSaver _saver;
    public void LoadStartScene()
    {
        _saver.Clear();
        StartCoroutine(LoadSceneWithDelay());
    }

    private IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Game");
    }
}

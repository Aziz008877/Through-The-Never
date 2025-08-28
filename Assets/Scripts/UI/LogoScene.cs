using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LogoScene : MonoBehaviour
{
    [SerializeField] private UIFade _uiFade;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        _uiFade.Fade(1);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Menu");
    }
}

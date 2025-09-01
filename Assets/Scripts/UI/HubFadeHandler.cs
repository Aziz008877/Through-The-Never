using System;
using System.Collections;
using UnityEngine;

public class HubFadeHandler : MonoBehaviour
{
    [SerializeField] private UIFade _fade;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(7.5f);
        _fade.Fade(0);
    }
}

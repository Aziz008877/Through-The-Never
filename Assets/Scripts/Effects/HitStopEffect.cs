using System.Collections;
using UnityEngine;
public class HitStopEffect : MonoBehaviour
{
    private bool _inStop;
    
    public void PlayStopEffect(float duration)
    {
        StartCoroutine(Apply(duration));
    }
    private IEnumerator Apply(float duration)
    {
        if (_inStop) yield break;
        
        _inStop = true;
        float tScale = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = tScale;
        _inStop = false;
    }
}
using System.Collections;
using UnityEngine;
public sealed class SlowMotionEffect : MonoBehaviour
{
    [SerializeField, Range(.1f, 1f)] private float _slowTimeScale = .35f;
    [SerializeField] private float _smooth = .05f;
    private Coroutine _slowMotionCoroutine;
    
    public void StartSlowMotion()
    {
        if (_slowMotionCoroutine != null)
        {
            StopCoroutine(_slowMotionCoroutine);
        }
        
        _slowMotionCoroutine = StartCoroutine(SetScale(_slowTimeScale));
    }
    
    public void EndSlowMotion()
    {
        if (_slowMotionCoroutine != null)
        {
            StopCoroutine(_slowMotionCoroutine);
        }
        
        _slowMotionCoroutine = StartCoroutine(SetScale(1f));
    }



    private IEnumerator SetScale(float target)
    {
        float start = Time.timeScale;
        
        for (float t = 0; t < _smooth; t += Time.unscaledDeltaTime)
        {
            Time.timeScale = Mathf.Lerp(start, target, t / _smooth);
            yield return null;
        }
        
        Time.timeScale = target;
        
        /*Time.timeScale = _slowTimeScale;
        yield return new WaitForSeconds(_slowDuration);
        Time.timeScale = 1;*/
    }
}
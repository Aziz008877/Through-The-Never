using UnityEngine;
public class FadeInOutEffect : MonoBehaviour
{
    [SerializeField] private UIFade _uiFade;
    private void Start()
    {
        _uiFade.Fade(0);
    }
}

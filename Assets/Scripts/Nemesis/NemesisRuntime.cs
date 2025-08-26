using UnityEngine;

public sealed class NemesisRuntime : MonoBehaviour
{
    [SerializeField] private NemesisConfig _config;
    public static NemesisService Svc { get; private set; }
    private void Awake()
    {
        if (Svc != null) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
        Svc = new NemesisService(_config);
    }
}
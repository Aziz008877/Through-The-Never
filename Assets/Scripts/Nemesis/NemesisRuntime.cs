using UnityEngine;

public sealed class NemesisRuntime : MonoBehaviour
{
    public static NemesisService Svc { get; private set; }

    [SerializeField] private NemesisConfig _config;

    private void Awake()
    {
        if (Svc != null) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
        Svc = new NemesisService(_config, new FileNemesisStorage());
    }
}
using UnityEngine;

public class EmissionPulseAndSpin : MonoBehaviour
{
    [Header("Emission (EV)")]
    [SerializeField] private Color emissionColor = Color.white; // базовый цвет
    [SerializeField] private float evMin = -5f;
    [SerializeField] private float evMax = 1f;
    [SerializeField] private float pulseHz = 1f;                // циклов в секунду
    [SerializeField] private bool randomizePhase = true;
    [SerializeField] private bool useUnscaledTime = false;

    [Header("Rotation")]
    [SerializeField] private float yDegreesPerSecond = 45f;
    private readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    private Renderer _renderer;
    private MaterialPropertyBlock _mpb;
    private float _phase;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
        if (randomizePhase) _phase = Random.value * Mathf.PI * 2f;
    }

    private void Update()
    {
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        float t = (useUnscaledTime ? Time.unscaledTime : Time.time);
        
        transform.Rotate(0f, yDegreesPerSecond * dt, 0f, Space.Self);
        
        float s = 0.5f + 0.5f * Mathf.Sin((t * pulseHz * 2f * Mathf.PI) + _phase);
        float ev = Mathf.Lerp(evMin, evMax, s);
        
        float intensity = Mathf.Pow(2f, ev);
        Color final = emissionColor * intensity;

        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(EmissionColorID, final);
        _renderer.SetPropertyBlock(_mpb);
    }
}

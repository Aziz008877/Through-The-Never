using System;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSequencePlayer : MonoBehaviour
{
    public enum LoopMode { Loop, PingPong, Once }

    [Header("Sequence")]
    [SerializeField] private Sprite[] _frames;
    [SerializeField] private float _fps = 12f;
    [SerializeField] private LoopMode _mode = LoopMode.Loop;
    [SerializeField] private bool _playOnAwake = true;
    [SerializeField] private bool _randomStartFrame = false;
    [SerializeField] private bool _useUnscaledTime = false;

    [Header("UI (optional)")]
    [SerializeField] private bool _setNativeSizeForUI = false;   // для Image: подгонять размер под спрайт

    public event Action OnSequenceCompleted; // срабатывает в режиме Once

    public bool IsPlaying { get; private set; }
    public int FrameIndex { get; private set; }

    float _timer;
    int _dir = 1; // для PingPong
    SpriteRenderer _sr;
    Image _img;

    void Awake()
    {
        _sr  = GetComponent<SpriteRenderer>();
        _img = GetComponent<Image>();

        if (_frames == null || _frames.Length == 0)
        {
            Debug.LogWarning($"{name}: SpriteSequencePlayer — пустой список кадров.");
            enabled = false;
            return;
        }

        if (_randomStartFrame) FrameIndex = UnityEngine.Random.Range(0, _frames.Length);
        ApplyFrame();

        if (_playOnAwake) Play();
        else Pause();
    }

    void Update()
    {
        if (!IsPlaying || _fps <= 0f) return;

        float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        _timer += dt;

        float frameDuration = 1f / _fps;
        while (_timer >= frameDuration) // корректно отрабатывает просадки
        {
            _timer -= frameDuration;
            StepFrame();
        }
    }

    void StepFrame()
    {
        switch (_mode)
        {
            case LoopMode.Loop:
                FrameIndex = (FrameIndex + 1) % _frames.Length;
                break;

            case LoopMode.PingPong:
                FrameIndex += _dir;
                if (FrameIndex >= _frames.Length - 1) { FrameIndex = _frames.Length - 1; _dir = -1; }
                else if (FrameIndex <= 0)             { FrameIndex = 0;                   _dir = 1;  }
                break;

            case LoopMode.Once:
                if (FrameIndex < _frames.Length - 1) FrameIndex++;
                else
                {
                    FrameIndex = _frames.Length - 1;
                    Pause();
                    OnSequenceCompleted?.Invoke();
                }
                break;
        }

        ApplyFrame();
    }

    void ApplyFrame()
    {
        var sprite = _frames[Mathf.Clamp(FrameIndex, 0, _frames.Length - 1)];

        if (_sr) _sr.sprite = sprite;

        if (_img)
        {
            _img.sprite = sprite;
            if (_setNativeSizeForUI) _img.SetNativeSize();
        }
    }

    // --- Публичный API ---
    public void Play()
    {
        if (_frames == null || _frames.Length == 0) return;
        IsPlaying = true;
    }

    public void Pause()   => IsPlaying = false;
    public void Stop()
    {
        IsPlaying = false;
        FrameIndex = 0;
        _dir = 1;
        _timer = 0f;
        ApplyFrame();
    }

    public void SetFrames(Sprite[] frames, bool autoPlay = true)
    {
        _frames = frames;
        FrameIndex = Mathf.Clamp(FrameIndex, 0, _frames.Length - 1);
        ApplyFrame();
        if (autoPlay) Play(); else Pause();
    }

    public void SetFPS(float fps) => _fps = Mathf.Max(0f, fps);
    public void SetLoopMode(LoopMode mode) => _mode = mode;
}

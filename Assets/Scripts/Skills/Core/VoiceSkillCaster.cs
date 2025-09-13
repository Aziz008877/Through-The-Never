using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceSkillCaster : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerSkillManager _skills;

    [Header("Recognition")]
    [SerializeField] private ConfidenceLevel _confidence = ConfidenceLevel.Low;
    [SerializeField] private KeyCode _activationKey = KeyCode.R; 
    [SerializeField] private float _globalCooldown = 0.25f;

    [Header("Optional aliases per slot")]
    [SerializeField] private SlotAliases[] _slotAliases = Array.Empty<SlotAliases>();

    [Serializable] public struct SlotAliases { public SkillSlot Slot; public string[] Aliases; }

    private KeywordRecognizer _recognizer;
    private readonly Dictionary<string, SkillSlot> _phraseToSlot = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<SkillSlot, HashSet<string>> _slotToPhrases = new();
    private float _lastCastTime;
    private bool _holdActive;
    private readonly Regex _sanitize = new(@"[^\p{L}\p{Nd}\s]", RegexOptions.Compiled);
    private readonly string[] _prefixes = { "cast ", "use ", "каст ", "применить ", "активируй " };
    private void OnEnable()
    {
        _skills.ActiveRegistered += OnActiveRegistered;
        RebuildPhrasesFromSkills();
        RebuildRecognizer();
    }

    private void OnDisable()
    {
        StopListening();
        DisposeRecognizer();
        _skills.ActiveRegistered -= OnActiveRegistered;
        _holdActive = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(_activationKey))
        {
            _holdActive = true;
            StartListening();
        }

        if (Input.GetKeyUp(_activationKey))
        {
            _holdActive = false;
            StopListening();
        }
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour behaviour)
    {
        bool wasListening = _recognizer != null && _recognizer.IsRunning;
        bool shouldResume = _holdActive && wasListening;

        RebuildPhrasesFromSkills();
        RebuildRecognizer();

        if (shouldResume) StartListening();
    }

    private void RebuildPhrasesFromSkills()
    {
        _phraseToSlot.Clear();
        _slotToPhrases.Clear();

        foreach (var kv in _skills.Actives)
        {
            var slot = kv.Key;
            var beh  = kv.Value;
            if (beh == null || beh.Definition == null) continue;

            EnsureSlot(slot);
            var main = Sanitize(RemoveKnownPrefixes(beh.Definition.DisplayName));
            AddPhraseForSlot(slot, main);

            var custom = _slotAliases.FirstOrDefault(a => a.Slot.Equals(slot));
            if (custom.Aliases != null)
                foreach (var alias in custom.Aliases)
                    AddPhraseForSlot(slot, Sanitize(RemoveKnownPrefixes(alias)));
        }
    }

    private void EnsureSlot(SkillSlot slot)
    {
        if (!_slotToPhrases.ContainsKey(slot))
            _slotToPhrases[slot] = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
    }

    private void AddPhraseForSlot(SkillSlot slot, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase)) return;
        if (_slotToPhrases[slot].Add(phrase))
            if (!_phraseToSlot.ContainsKey(phrase))
                _phraseToSlot.Add(phrase, slot);
    }

    private void RebuildRecognizer()
    {
        DisposeRecognizer();
        var keywords = _phraseToSlot.Keys.Where(k => !string.IsNullOrWhiteSpace(k)).Distinct().ToArray();
        if (keywords.Length == 0)
        {
            Debug.LogWarning("[VoiceSkillCaster] Нет фраз для распознавания (нет активных навыков?).");
            return;
        }
        _recognizer = new KeywordRecognizer(keywords, _confidence);
        _recognizer.OnPhraseRecognized += OnPhraseRecognized;
    }

    private void DisposeRecognizer()
    {
        if (_recognizer != null)
        {
            _recognizer.OnPhraseRecognized -= OnPhraseRecognized;
            if (_recognizer.IsRunning) _recognizer.Stop();
            _recognizer.Dispose();
            _recognizer = null;
        }
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (!_holdActive) return;
        if (Time.unscaledTime - _lastCastTime < _globalCooldown) return;

        var heard = Sanitize(RemoveKnownPrefixes(args.text));
        if (_phraseToSlot.TryGetValue(heard, out var slot))
        {
            if (_skills.Actives.TryGetValue(slot, out var beh) && beh != null)
            {
                var cast = typeof(PlayerSkillManager)
                    .GetMethod("Cast", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new[] { typeof(SkillSlot) }, null);
                if (cast != null)
                {
                    cast.Invoke(_skills, new object[] { slot });
                    _lastCastTime = Time.unscaledTime;
                    Debug.Log($"[VoiceSkillCaster] Voice '{args.text}' → CAST {slot} (conf: {args.confidence})");
                }
                else
                {
                    Debug.LogWarning("[VoiceSkillCaster] Не нашли PlayerSkillManager.Cast(SkillSlot). Сделай её public — и вызову напрямую.");
                }
            }
        }
        else
        {
            Debug.Log($"[VoiceSkillCaster] Фраза не найдена среди доступных навыков: {args.text}");
        }
    }

    public void StartListening()
    {
        if (_recognizer != null && !_recognizer.IsRunning) _recognizer.Start();
    }

    public void StopListening()
    {
        if (_recognizer != null && _recognizer.IsRunning) _recognizer.Stop();
    }

    private string RemoveKnownPrefixes(string phrase)
    {
        if (string.IsNullOrEmpty(phrase)) return phrase;
        var lower = phrase.ToLowerInvariant();
        foreach (var p in _prefixes)
            if (lower.StartsWith(p)) return phrase.Substring(p.Length);
        return phrase;
    }

    private string Sanitize(string phrase)
    {
        if (string.IsNullOrEmpty(phrase)) return string.Empty;
        phrase = phrase.ToLowerInvariant();
        phrase = _sanitize.Replace(phrase, "");
        phrase = Regex.Replace(phrase, @"\s+", " ").Trim();
        return phrase;
    }
}

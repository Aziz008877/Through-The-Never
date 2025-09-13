using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Skill Definition")]
public class SkillDefinition : ScriptableObject
{
    [Header("General Data")]
    public string Id;
    public string DisplayName;
    public string Description;
    public Sprite Icon;
    public AudioClip SkillSound;
    [Header("Aspects (stars)")]
    public AspectStars[] Stars;
    public SkillSlot Slot;
    public SkillKind Kind;
    public MagicSchool School;

    [Header("Prefab Behaviour")]
    public GameObject BehaviourPrefab;
    
    [Header("Cast Settings")]
    public CastType CastType;

    [Header("Data")]
    public float Damage;
    public float Cooldown;
    public float Duration;
    public float Range;
    public float Raduis;
}
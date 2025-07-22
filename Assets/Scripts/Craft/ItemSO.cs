using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Crafting/Item")]
public class ItemSO : ScriptableObject
{
    public string DisplayName;
    public Sprite Icon;
    public ItemRole Role;
    public UseBehaviour Behaviour;
    public SkillBehaviour Skill;
    public int Charges = 1;
}

public enum ItemRole { Remnant, Charm, Scroll, Stone }
public enum UseBehaviour { None, Consumable, PassivePermanent, PassiveSingleTrigger }
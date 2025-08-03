using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Crafting/Item")]
public class ItemSO : ScriptableObject
{
    public string DisplayName;
    public Sprite Icon;
    public ItemRole Role;
    public UseBehaviour Behaviour;
    //public SkillBehaviour Skill;
    public SkillDefinition Skill;
    public int Charges = 1;
    
    [Tooltip("Только для Role = Charm")]
    public CharmSO CharmRef;
}

public enum ItemRole { Remnant, Charm, Scroll, Stone }
public enum UseBehaviour { None, Consumable, PassivePermanent, PassiveSingleTrigger }
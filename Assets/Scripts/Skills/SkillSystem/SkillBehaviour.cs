using UnityEngine;

public abstract  class SkillBehaviour : MonoBehaviour
{
    public SkillDefinition Definition { get; private set; }
    protected PlayerContext PlayerContext { get; private set; }

    public virtual void Inject(SkillDefinition definition, PlayerContext context)
    {
        Definition = definition;
        PlayerContext = context;
    }
}

public abstract class ActiveSkillBehaviour : SkillBehaviour
{
    public abstract bool IsReady { get; }
    public abstract void TryCast();
}

public abstract class PassiveSkillBehaviour : SkillBehaviour
{
    public abstract void Enable();
    public abstract void Disable();
}

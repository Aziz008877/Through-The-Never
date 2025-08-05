using UnityEngine;

public class SkillRuntimeFactory
{
    public SkillBehaviour Spawn(SkillDefinition definition, ActorContext context, Transform parent)
    {
        var go = Object.Instantiate(definition.BehaviourPrefab, parent);

        if (go.TryGetComponent(out SkillBehaviour skillBehaviour))
        {
            skillBehaviour.Inject(definition, context);
            return skillBehaviour;
        }

        return null;
    }
}
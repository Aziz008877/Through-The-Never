using UnityEngine;

public static class SkillExtensions
{
    public static void TryCastByDefinition(this ActiveSkillBehaviour skill, Transform target)
    {
        if (!skill || !skill.IsReady) return;

        Vector3 point = target != null ? target.position : skill.transform.position;

        switch (skill.Definition.CastType)
        {
            case CastType.OnSelf:
                RotateOwner(skill, skill.transform.position + skill.transform.forward);
                skill.TryCast();
                break;

            case CastType.AtTarget:
                if (target == null) return;
                RotateOwner(skill, target.position);
                skill.TryCast();
                break;

            case CastType.AtPoint:
                RotateOwner(skill, point);
                skill.TryCast();
                break;
        }
    }


    private static void RotateOwner(ActiveSkillBehaviour skill, Vector3 lookPoint)
    {
        Transform owner = skill.transform;
        Vector3 dir = lookPoint - owner.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        owner.rotation = Quaternion.LookRotation(dir);
    }
}
using UnityEngine;

public static class SkillExtensions
{
    public static void TryCastAtTarget(this ActiveSkillBehaviour skill, Transform target)
    {
        if (!skill || !skill.IsReady || target == null) return;
        RotateOwner(skill, target.position);
        skill.TryCast();
    }

    public static void TryCastAtPoint(this ActiveSkillBehaviour skill, Vector3 point)
    {
        if (!skill || !skill.IsReady) return;
        RotateOwner(skill, point);
        skill.TryCast();
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
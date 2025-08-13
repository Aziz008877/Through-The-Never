using UnityEngine;

public class AspectOfTiamatSkill : ActiveSkillBehaviour
{
    [SerializeField] private TiamatAvatar _avatarPrefab;

    public override void TryCast()
    {
        if (!IsReady) return;

        if (!TryGetAimPoint(out var spawnPosition)) return;

        var avatar = Instantiate(_avatarPrefab, spawnPosition, Quaternion.identity);
        avatar.Initialize(Context, Damage, Radius, Duration);

        StartCooldown();
        OnSkillActivated?.Invoke(Duration);
    }

    private bool TryGetAimPoint(out Vector3 point)
    {
        var camera = Context.MainCamera ? Context.MainCamera : Camera.main;
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            point = hit.point;
            return true;
        }
        point = Vector3.zero;
        return false;
    }
}
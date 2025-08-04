using UnityEngine;
using Zenject;

[RequireComponent(typeof(CompanionContext), typeof(CompanionMove))]
public abstract class CompanionControllerBase : MonoBehaviour
{
    /* ───────── Follow-настройки ───────── */
    [Header("Follow")]
    [Tooltip("Если дистанция > Follow Radius, начинаем догонять игрока")]
    [SerializeField] float _followRadius = 6f;

    [Tooltip("Если дистанция ≤ Stop Distance, компаньон полностью останавливается")]
    [SerializeField] float _stopDist = 1.3f;

    [Tooltip("Секунды без выстрелов игрока, после чего компаньон сам выбирает цель")]
    [SerializeField] float _idleDelay = 2f;

    /* ───────── Поиск врагов ───────── */
    [Header("Enemy Search")]
    [SerializeField] float _searchRadius = 12f;

    [Tooltip("Слой, на котором находятся враги")]
    [SerializeField] LayerMask _enemyMask = ~0; // «все», если забыли настроить

    /* ───────── Ссылки ───────── */
    protected CompanionContext Ctx;
    CompanionMove _move;

    [Inject] private PlayerContext _player;   // позиция игрока

    /* ───────── Состояние ───────── */
    Vector3 _lastShotPoint;
    float   _lastShotTime = -999f;

    /* ───────── Жизненный цикл ───────── */
    protected virtual void Awake()
    {
        Ctx   = GetComponent<CompanionContext>();
        _move = GetComponent<CompanionMove>();

        PlayerBasicAttackEvents.OnBasicAttack += SaveShot;
    }
    protected virtual void OnDestroy() =>
        PlayerBasicAttackEvents.OnBasicAttack -= SaveShot;

    void Update()
    {
        FollowPlayer();
        CombatLoop();                       // реализация в потомке
    }

    /* ───────── Follow ───────── */
    void FollowPlayer()
    {
        float dist = Vector3.Distance(transform.position, _player.transform.position);

        if (dist > _followRadius)           // далеко → догоняем
        {
            _move.MoveTo(_player.transform.position);
        }
        else                                // в комфортной зоне → стоим
        {
            if (dist <= _stopDist) _move.Stop();
            else                   _move.Stop();   // «коридор» между stop и follow
        }
    }

    /* ───────── Событие выстрела игрока ───────── */
    void SaveShot(Vector3 p)
    {
        _lastShotPoint = p;
        _lastShotTime  = Time.time;
    }
    protected bool    PlayerIsActive => Time.time - _lastShotTime <= _idleDelay;
    protected Vector3 LastShotPoint  => _lastShotPoint;

    /* ───────── Поиск ближайшего врага ───────── */
    protected Transform NearestEnemy()
    {
        Transform best = null;
        float     min  = float.MaxValue;

        foreach (var c in Physics.OverlapSphere(transform.position, _searchRadius, _enemyMask))
        {
            float d = (c.transform.position - transform.position).sqrMagnitude;
            if (d < min) { min = d; best = c.transform; }
        }
        return best;
    }

    /* ───────── Реализуется наследником ───────── */
    protected abstract void CombatLoop();
}

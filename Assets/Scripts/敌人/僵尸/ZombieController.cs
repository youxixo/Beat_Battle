using UnityEngine;
using UnityHFSM;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private ZombieDate zombieDate;
    [SerializeField] private Animator animator;
    
    [Header("待机动画")]
    [SerializeField] private string IdleAnimName = "Idle";

    [Header("移动动画")]
    [SerializeField] private string MoveAnimName = "Move";

    [Header("攻击")]
    [Header("碰撞箱")]
    [SerializeField] private Collider[] AttackBoxCollider;

    [Header("动画")]
    [SerializeField] private string AttackAnimName = "Attack";

    [Header("死亡动画")]
    [SerializeField] private string DeathAnimName = "Death";

    private StateMachine<ZombieType> zombieStateMachine = new();

    private MultiTimerManager timerManager => MultiTimerManager.Instance;
    private CharacterManager characterManager => CharacterManager.Instance;
    private BeatManager beatManager => BeatManager.Instance;

    private DownTimer AttackCooldownTimer;

    private bool CandoSomething = true;

    private void Awake()
    {
        foreach (var collider in AttackBoxCollider)
        {
            collider.enabled = false;
        }
        AttackCooldownTimer = timerManager.Create_DownTimer("Zombie_Attack_Cooldown" + gameObject.GetInstanceID());

        Build_ZombieStateMachine(); 
    }

    void OnEnable()
    {
        beatManager.StartBeatCheckAction += Stop;
        beatManager.StopBeatCheckAction += Resume;
        zombieStateMachine.Init();
    }

    void Update()
    {
        if (CandoSomething)
        {
            zombieStateMachine.OnLogic();
        }
    }

    void OnDisable()
    {
        zombieStateMachine.OnExit();

        if (beatManager != null)
        {
            beatManager.StartBeatCheckAction -= Stop;
            beatManager.StopBeatCheckAction -= Resume;
        }
    }

    private void Build_ZombieStateMachine()
    {
        var idleState = new Idle_Zombie(animator, IdleAnimName);
        var moveState = new Move_Zombie(animator, MoveAnimName, zombieDate);
        var attaclState = new Attack_Zombie(animator, AttackAnimName, AttackBoxCollider, AttackCooldownTimer, zombieDate);
        var dieState = new Die_Zombie(animator, DeathAnimName, zombieDate);
        
        zombieStateMachine.AddState(ZombieType.Idle, idleState);
        zombieStateMachine.AddState(ZombieType.Move, moveState);
        zombieStateMachine.AddState(ZombieType.Attack, attaclState);
        zombieStateMachine.AddState(ZombieType.Die, dieState);

        // Idle -> Attack
        zombieStateMachine.AddTransition(ZombieType.Idle, ZombieType.Attack, t => AttackCooldownTimer.IsComplete() && IsPlayerInAttackRange());
        // Idle -> Move
        zombieStateMachine.AddTransition(ZombieType.Idle, ZombieType.Move, t => zombieDate.CanMove && !IsPlayerInAttackRange());
        
        //move -> Attack
        zombieStateMachine.AddTransition(ZombieType.Move, ZombieType.Attack, t => IsPlayerInAttackRange() && AttackCooldownTimer.IsComplete());
        //move -> Idle
        zombieStateMachine.AddTransition(ZombieType.Move, ZombieType.Idle, t => IsPlayerInAttackRange());

        // Attack -> Move
        zombieStateMachine.AddTransition(ZombieType.Attack, ZombieType.Move, t => !IsPlayerInAttackRange());
        // Attack -> Idle
        zombieStateMachine.AddTransition(ZombieType.Attack, ZombieType.Idle);

        zombieStateMachine.AddTransitionFromAny(ZombieType.Die, t => zombieDate.IsDead);

        zombieStateMachine.SetStartState(ZombieType.Idle);
    }

    private bool IsPlayerInAttackRange()
    {
        var player = CharacterManager.Instance.GetCurrentCharacterData;
        if (player == null)
        {
            return false;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        return distanceToPlayer <= zombieDate.AttackRange;
    }

    private void Stop()
    {
        animator.speed = 0;
        CandoSomething = false;
    }

    private void Resume()
    {
        animator.speed = 1;
        CandoSomething = true;
    }

    /// <summary>
    /// 受伤逻辑
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitBox"))
        {
            bool isAttack3 = characterManager.GetCurrentCharacterData.GetCurrentLandAttackType == LandAttackType.LandAttack3_Attack;
            if(isAttack3)
            {
                zombieDate.TakeDamage(5);
            }
            else
            {
                zombieDate.TakeDamage(1);
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    private void OnValidate()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        if(zombieDate == null)
        {
            zombieDate = GetComponent<ZombieDate>();
        }
    }
    #endif
}

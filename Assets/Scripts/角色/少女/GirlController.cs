using UnityEngine;
using UnityHFSM;

public class GirlController : MonoBehaviour
{
    [SerializeField] private Girl_Data girlData;
    [Header("移动组件")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GirlCheckGround girlCheckGround;

    [Header("动画组件")]
    [SerializeField] private Animator animator;

    [Header("待机动画")]
    [SerializeField] private string IdleAnimName = "Idle";

    [Header("跑步动画")]
    [SerializeField] private string RunAnimName = "Run";

    [Header("跳跃动画")]
    [SerializeField] private string JumpUpAnimName = "JumpUp";
    [SerializeField] private string JumpDownAnimName = "JumpDown";
    [SerializeField] private string JumpLandAnimName = "JumpLand";

    [Header("地面攻击")]
    [SerializeField] private Collider LandAttackCollider;

    [Header("招式1")]
    [SerializeField] private SpherePainter LandAttack1MaxMoveDistanceRadius;
    [SerializeField] private SpherePainter LandAttack1Radius;
    [Header("动画")]
    [SerializeField] private string Attack1StartAnimName = "Attack1Start";
    [SerializeField] private string Attack1WorkingAnimName = "Attack1Working";
    [SerializeField] private string Attack1EndAnimName = "Attack1End";

    [Header("招式2")]
    [SerializeField] private SpherePainter LandAttack2MaxMoveDistanceRadius;
    [SerializeField] private SpherePainter LandAttack2Radius;
    [Header("动画")]
    [SerializeField] private string Attack2StartAnimName = "Attack2Start";
    [SerializeField] private string Attack2WorkingAnimName = "Attack2Working";
    [SerializeField] private string Attack2EndAnimName = "Attack2End";

    [Header("招式3")]
    [SerializeField] private SpherePainter LandAttack3MaxMoveDistanceRadius;
    [SerializeField] private SpherePainter LandAttack3Radius;
    [Header("动画")]
    [SerializeField] private string Attack3StartAnimName = "Attack3Start";
    [SerializeField] private string Attack3WorkingAnimName = "Attack3Working";
    [SerializeField] private string Attack3EndAnimName = "Attack3End";


    private StateMachine<GirlStateType, InputEvent> GirlRootStateMachine= new();
    private StateMachine<GirlStateType,JumpUpType,InputEvent> JumpUpStateMachine = new();
    private StateMachine<GirlStateType, LandAttackType, InputEvent> LandAttackStateMachine = new();

    private bool isGround => girlCheckGround.GetIsGround;

    private InputManager inputManager => InputManager.Instance;
    private CharacterManager characterManager => CharacterManager.Instance;

    private Vector3 moveDirection => inputManager.GetMoveDirection;

    private GirlStateType currentState => GirlRootStateMachine.ActiveStateName;

    private void Awake()
    {
        characterManager.SetCurrentCharacterData(girlData);

        Build_JumpUpMachine();
        Build_LandAttackMachine();


        Build_RootMachine();
    }

    void OnEnable()
    {
        inputManager.AttackTapEvent += Tap_Attack;
        inputManager.AttackHoldEvent += Hold_Attack;
        inputManager.JumpEvent += Jump;

        LandAttackCollider.enabled = false;
        GirlRootStateMachine.Init();
    }

    void Update()
    {
        GirlRootStateMachine.OnLogic();
    }

    void OnDisable()
    {
        GirlRootStateMachine.OnExit();

        if (inputManager != null)
        {
            inputManager.AttackTapEvent -= Tap_Attack;
            inputManager.AttackHoldEvent -= Hold_Attack;
            inputManager.JumpEvent -= Jump;
        }
    }

    void OnDestroy()
    {
        GirlRootStateMachine.OnExit();

        if (inputManager != null)
        {
            inputManager.AttackTapEvent -= Tap_Attack;
            inputManager.AttackHoldEvent -= Hold_Attack;
            inputManager.JumpEvent -= Jump;
        }
    }

    private void Build_RootMachine()
    {
        GirlRootStateMachine.AddState(GirlStateType.WaitingInput, new WaitingInput());
        GirlRootStateMachine.AddState(GirlStateType.Idle, new Idle_Girl(animator, Animator.StringToHash(IdleAnimName)));
        GirlRootStateMachine.AddState(GirlStateType.Run, new Run_Girl(characterController, girlData, 
                                                                        animator, Animator.StringToHash(RunAnimName)));
        GirlRootStateMachine.AddState(GirlStateType.JumpUpMachine, JumpUpStateMachine);
        GirlRootStateMachine.AddState(GirlStateType.JumpDown, new Jump_Down_Girl(girlData, animator, Animator.StringToHash(JumpDownAnimName), characterController));
        GirlRootStateMachine.AddState(GirlStateType.JumpLand, new Jump_Land_Girl(animator, Animator.StringToHash(JumpLandAnimName)));
        GirlRootStateMachine.AddState(GirlStateType.LandAttackMachine, LandAttackStateMachine);

        // 待输入状态切换
        GirlRootStateMachine.AddTriggerTransition(InputEvent.Jump, GirlStateType.WaitingInput, GirlStateType.JumpUpMachine,t => girlData.CanJump);
        GirlRootStateMachine.AddTriggerTransition(InputEvent.AttackTap, GirlStateType.WaitingInput, GirlStateType.LandAttackMachine, t => isGround);
        GirlRootStateMachine.AddTransition(GirlStateType.WaitingInput, GirlStateType.JumpDown, t => !isGround);
        GirlRootStateMachine.AddTransition(GirlStateType.WaitingInput, GirlStateType.Run, t=> isGround && moveDirection != Vector3.zero);
        GirlRootStateMachine.AddTransition(GirlStateType.WaitingInput, GirlStateType.Idle, t => isGround);

        //待机状态切换
        GirlRootStateMachine.AddTransition(GirlStateType.Idle, GirlStateType.WaitingInput, t => !isGround || moveDirection != Vector3.zero);

        //跑步状态切换
        GirlRootStateMachine.AddTransition(GirlStateType.Run, GirlStateType.WaitingInput, t => !isGround || moveDirection == Vector3.zero);

        //跳跃上升状态切换
        GirlRootStateMachine.AddTransition(GirlStateType.JumpUpMachine, GirlStateType.WaitingInput, t => girlData.GetCurrentYVelocity <=0f);

        //跳跃下降状态切换
        GirlRootStateMachine.AddTransition(GirlStateType.JumpDown, GirlStateType.JumpLand, t => isGround);

        //跳跃落地状态切换
        GirlRootStateMachine.AddTransition(GirlStateType.JumpLand, GirlStateType.WaitingInput, 
                                            t => moveDirection != Vector3.zero, forceInstantly: true);
        GirlRootStateMachine.AddTransition(GirlStateType.JumpLand, GirlStateType.WaitingInput);

        //地面攻击状态切换
        GirlRootStateMachine.AddTransition(GirlStateType.LandAttackMachine, GirlStateType.WaitingInput, t => !girlData.GetIsLandAttacking);
        GirlRootStateMachine.AddTransition(GirlStateType.LandAttackMachine, GirlStateType.WaitingInput, 
                                            t => moveDirection != Vector3.zero && inputManager.GetMoveInputWindow &&
                                            LandAttackStateMachine.ActiveStateName == LandAttackType.LandAttack1_End);
        GirlRootStateMachine.AddTransition(GirlStateType.LandAttackMachine, GirlStateType.WaitingInput, 
                                            t => moveDirection != Vector3.zero && inputManager.GetMoveInputWindow &&
                                            LandAttackStateMachine.ActiveStateName == LandAttackType.LandAttack2_End, forceInstantly: true);
        GirlRootStateMachine.AddTransition(GirlStateType.LandAttackMachine, GirlStateType.WaitingInput, 
                                            t => moveDirection != Vector3.zero && inputManager.GetMoveInputWindow &&
                                            LandAttackStateMachine.ActiveStateName == LandAttackType.LandAttack3_End, forceInstantly: true);

        //任意状态切换
        GirlRootStateMachine.AddTriggerTransitionFromAny(InputEvent.Jump, GirlStateType.JumpUpMachine, 
                                            t => girlData.CanJump && currentState!= GirlStateType.JumpUpMachine, forceInstantly: true);
        GirlRootStateMachine.AddTriggerTransitionFromAny(InputEvent.AttackTap, GirlStateType.LandAttackMachine, 
                                            t => isGround && currentState != GirlStateType.LandAttackMachine, forceInstantly: true);
        
        
        GirlRootStateMachine.SetStartState(GirlStateType.WaitingInput);
    }

    private void Build_JumpUpMachine()
    {
        JumpUpStateMachine.AddState(JumpUpType.JumpUp_Enter, new JumpUp_Enter_Girl(girlData));
        JumpUpStateMachine.AddState(JumpUpType.Jump_Up, new Jump_Up_Girl(girlData, characterController, 
                                                                        animator, Animator.StringToHash(JumpUpAnimName)));
        
        JumpUpStateMachine.AddTransition(JumpUpType.JumpUp_Enter, JumpUpType.Jump_Up);
        JumpUpStateMachine.AddTriggerTransition(InputEvent.Jump, JumpUpType.Jump_Up, JumpUpType.JumpUp_Enter, 
                                                t => girlData.CanJump && girlData.GetCurrentYVelocity <=1f);

        JumpUpStateMachine.AddExitTransition(JumpUpType.Jump_Up, t => girlData.GetCurrentYVelocity <=0f);
        JumpUpStateMachine.SetStartState(JumpUpType.JumpUp_Enter);
    }

    private void Build_LandAttackMachine()
    {
        LandAttackType nextLandAttack = girlData.NextLandAttackType;

        LandAttackStateMachine.AddState(LandAttackType.LandAttack_Enter, new LandAttack_Enter(girlData));

        // 地面攻击1状态
        LandAttackStateMachine.AddState(LandAttackType.LandAttack1_Start, new LandAttack_Start(girlData, characterController, animator, 
                                                                    Animator.StringToHash(Attack1StartAnimName), LandAttackType.LandAttack2_Start, 
                                                                    LandAttack1Radius.GetRadius, LandAttack1MaxMoveDistanceRadius.GetRadius));
        LandAttackStateMachine.AddState(LandAttackType.LandAttack1_Attack, new LandAttack_Attack(girlData, animator, 
                                                                    Animator.StringToHash(Attack1WorkingAnimName), LandAttackCollider));
        LandAttackStateMachine.AddState(LandAttackType.LandAttack1_End, new LandAttack_End(girlData, animator, 
                                                                    Animator.StringToHash(Attack1EndAnimName), 1f));

        // 地面攻击2状态
        LandAttackStateMachine.AddState(LandAttackType.LandAttack2_Start, new LandAttack_Start(girlData, characterController, animator, 
                                                                    Animator.StringToHash(Attack2StartAnimName), LandAttackType.LandAttack3_Start, 
                                                                    LandAttack2Radius.GetRadius, LandAttack2MaxMoveDistanceRadius.GetRadius));
        LandAttackStateMachine.AddState(LandAttackType.LandAttack2_Attack, new LandAttack_Attack(girlData, animator, 
                                                                    Animator.StringToHash(Attack2WorkingAnimName), LandAttackCollider));
        LandAttackStateMachine.AddState(LandAttackType.LandAttack2_End, new LandAttack_End(girlData, animator, 
                                                                    Animator.StringToHash(Attack2EndAnimName), 1f));

        // 地面攻击3状态
        LandAttackStateMachine.AddState(LandAttackType.LandAttack3_Start, new LandAttack_Start(girlData, characterController, animator, 
                                                                    Animator.StringToHash(Attack3StartAnimName), LandAttackType.LandAttack1_Start, 
                                                                    LandAttack3Radius.GetRadius, LandAttack3MaxMoveDistanceRadius.GetRadius));
        LandAttackStateMachine.AddState(LandAttackType.LandAttack3_Attack, new LandAttack_Attack(girlData, animator, 
                                                                    Animator.StringToHash(Attack3WorkingAnimName), LandAttackCollider));
        LandAttackStateMachine.AddState(LandAttackType.LandAttack3_End, new LandAttack_End(girlData, animator, 
                                                                    Animator.StringToHash(Attack3EndAnimName), 1f));     

        //转换
        LandAttackStateMachine.AddTransition(LandAttackType.LandAttack_Enter, LandAttackType.LandAttack1_Start, 
                                            t => nextLandAttack == LandAttackType.LandAttack1_Start);
        LandAttackStateMachine.AddTransition(LandAttackType.LandAttack_Enter, LandAttackType.LandAttack2_Start, 
                                            t => nextLandAttack == LandAttackType.LandAttack2_Start);
        LandAttackStateMachine.AddTransition(LandAttackType.LandAttack_Enter, LandAttackType.LandAttack3_Start, 
                                            t => nextLandAttack == LandAttackType.LandAttack3_Start);

        //地面攻击1
            // 起手 -> 攻击
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack1_Start, LandAttackType.LandAttack1_Attack);
            // 攻击1 -> 攻击2
            LandAttackStateMachine.AddTriggerTransition(InputEvent.AttackTap, LandAttackType.LandAttack1_Attack, 
                                                        LandAttackType.LandAttack2_Start, forceInstantly: true);
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack1_Attack, LandAttackType.LandAttack2_Start, 
                            t => inputManager.AttackExpire && inputManager.GetAttackInputWindow, forceInstantly: true);
            //攻击 -> 结束
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack1_Attack, LandAttackType.LandAttack1_End);

            // 结束 -> 攻击2
            LandAttackStateMachine.AddTriggerTransition(InputEvent.AttackTap, LandAttackType.LandAttack1_End, 
                                                    LandAttackType.LandAttack2_Start, forceInstantly: true);
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack1_End, LandAttackType.LandAttack2_Start, 
                            t => inputManager.AttackExpire && inputManager.GetAttackInputWindow, forceInstantly: true);

        //地面攻击2
            // 起手 -> 攻击
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack2_Start, LandAttackType.LandAttack2_Attack);
            //攻击 -> 结束
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack2_Attack, LandAttackType.LandAttack2_End);

            // 攻击2 -> 攻击3
            LandAttackStateMachine.AddTriggerTransition(InputEvent.AttackTap, LandAttackType.LandAttack2_Attack, 
                                                        LandAttackType.LandAttack3_Start, forceInstantly: true);
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack2_Attack, LandAttackType.LandAttack3_Start, 
                            t => inputManager.AttackExpire && inputManager.GetAttackInputWindow, forceInstantly: true);
            // 攻击 2 -> 结束
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack2_Attack, LandAttackType.LandAttack2_End);

            // 结束 -> 攻击3
            LandAttackStateMachine.AddTriggerTransition(InputEvent.AttackTap, LandAttackType.LandAttack2_End, 
                                                        LandAttackType.LandAttack3_Start, forceInstantly: true);
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack2_End, LandAttackType.LandAttack3_Start, 
                            t => inputManager.AttackExpire && inputManager.GetAttackInputWindow, forceInstantly: true);

            
        //地面攻击3
            // 起手 -> 攻击
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack3_Start, LandAttackType.LandAttack3_Attack);

            //攻击3 -> 攻击1
            LandAttackStateMachine.AddTriggerTransition(InputEvent.AttackTap, LandAttackType.LandAttack3_Attack, 
                                                        LandAttackType.LandAttack1_Start, forceInstantly: true);
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack3_Attack, LandAttackType.LandAttack1_Start, 
                            t => inputManager.AttackExpire && inputManager.GetAttackInputWindow, forceInstantly: true);
            //攻击3 -> 结束
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack3_Attack, LandAttackType.LandAttack3_End);

            // 结束 -> enter
            LandAttackStateMachine.AddTriggerTransition(InputEvent.AttackTap, LandAttackType.LandAttack3_End, 
                                                        LandAttackType.LandAttack1_Start, forceInstantly: true);
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack3_End, LandAttackType.LandAttack1_Start, 
                            t => inputManager.AttackExpire && inputManager.GetAttackInputWindow, forceInstantly: true);


        LandAttackStateMachine.SetStartState(LandAttackType.LandAttack_Enter);
    }

    /// <summary>
    /// 点击攻击键
    /// </summary>
    void Tap_Attack()
    {
        GirlRootStateMachine.Trigger(InputEvent.AttackTap);
    }

    void Hold_Attack()
    {
        GirlRootStateMachine.Trigger(InputEvent.AttackHold);
    }

    void Jump()
    {
        GirlRootStateMachine.Trigger(InputEvent.Jump);
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
        if (girlCheckGround == null)
        {
            girlCheckGround = GetComponentInChildren<GirlCheckGround>();
        }
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (girlData == null)
        {
            girlData = GetComponent<Girl_Data>();
        }
    }
    #endif

}

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
    [SerializeField] private string IdleAnimName = "Idle";
    [SerializeField] private string RunAnimName = "Run";
    [SerializeField] private string JumpUpAnimName = "JumpUp";
    [SerializeField] private string JumpDownAnimName = "JumpDown";
    [SerializeField] private string JumpLandAnimName = "JumpLand";

    private StateMachine<GirlStateType, InputEvent> GirlRootStateMachine= new();
    private StateMachine<GirlStateType,JumpUpType,InputEvent> JumpUpStateMachine = new();

    private bool isGround => girlCheckGround.GetIsGround;

    private InputManager inputManager => InputManager.Instance;

    private Vector2 moveDirection => inputManager.GetMoveDirection;

    private GirlStateType currentState => GirlRootStateMachine.ActiveStateName;

    private void Awake()
    {
        Build_JumpUpMachine();
        Build_RootMachine();
    }

    void OnEnable()
    {
        inputManager.AttackTapEvent += Tap_Attack;
        inputManager.AttackHoldEvent += Hold_Attack;
        inputManager.JumpEvent += Jump;

        GirlRootStateMachine.Init();
    }

    void Update()
    {
        GirlRootStateMachine.OnLogic();
        Debug.Log($"当前状态：{GirlRootStateMachine.GetActiveHierarchyPath()}");
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
        GirlRootStateMachine.AddState(GirlStateType.Run, new Run_Girl(characterController, girlData.GetMoveSpeed, 
                                                                        animator, Animator.StringToHash(RunAnimName)));                                                      
        GirlRootStateMachine.AddState(GirlStateType.JumpUpMachine, JumpUpStateMachine);
        GirlRootStateMachine.AddState(GirlStateType.JumpDown, new Jump_Down_Girl(girlData, animator, Animator.StringToHash(JumpDownAnimName), characterController));
        GirlRootStateMachine.AddState(GirlStateType.JumpLand, new Jump_Land_Girl(animator, Animator.StringToHash(JumpLandAnimName)));

        // 待输入状态切换
        GirlRootStateMachine.AddTriggerTransition(InputEvent.Jump, GirlStateType.WaitingInput, GirlStateType.JumpUpMachine,t => girlData.CanJump);
        GirlRootStateMachine.AddTransition(GirlStateType.WaitingInput, GirlStateType.JumpDown, t => !isGround);
        GirlRootStateMachine.AddTransition(GirlStateType.WaitingInput, GirlStateType.Run, t=> isGround && moveDirection != Vector2.zero);
        GirlRootStateMachine.AddTransition(GirlStateType.WaitingInput, GirlStateType.Idle, t => isGround);

        //待机状态切换
        GirlRootStateMachine.AddTransition(GirlStateType.Idle, GirlStateType.WaitingInput, t => !isGround || moveDirection != Vector2.zero);

        //跑步状态切换
        GirlRootStateMachine.AddTransition(GirlStateType.Run, GirlStateType.WaitingInput, t => !isGround || moveDirection == Vector2.zero);

        //跳跃上升状态切换
        GirlRootStateMachine.AddTransition(GirlStateType.JumpUpMachine, GirlStateType.WaitingInput, t => girlData.GetCurrentYVelocity <=0f);

        //跳跃下降状态切换
        GirlRootStateMachine.AddTransition(GirlStateType.JumpDown, GirlStateType.JumpLand, t => isGround);

        //跳跃落地状态切换
        GirlRootStateMachine.AddTransition(GirlStateType.JumpLand, GirlStateType.WaitingInput);

        //任意状态切换
        GirlRootStateMachine.AddTriggerTransitionFromAny(InputEvent.Jump, GirlStateType.JumpUpMachine, 
                                                            t => girlData.CanJump && currentState!= GirlStateType.JumpUpMachine, forceInstantly: true);
        
        
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

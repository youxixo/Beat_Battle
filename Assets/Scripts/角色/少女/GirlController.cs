using UnityEngine;
using UnityHFSM;

public class GirlController : MonoBehaviour
{
    [SerializeField] private Girl_Data girlData;
    [Header("移动组件")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GirlCheckGround girlCheckGround;

    [Header("音频组件")]
    [SerializeField] private AudioSource audioSource;

    [Header("动画组件")]
    [SerializeField] private Animator animator;

    [Header("待机动画")]
    [SerializeField] private string IdleAnimName = "Idle";

    [Header("跑步动画")]
    [SerializeField] private string RunAnimName = "Run";

    [Header("闪避动画")]
    [SerializeField] private string Dodge_Front_AnimName = "Dodge_Front";
    [SerializeField] private string Dodge_Back_AnimName = "Dodge_Back";

    [Header("闪避音效")]
    [SerializeField] private AudioClip F_DodgeAudioClip;
    [SerializeField] private AudioClip A_DodgeAudioClip;

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
    [Header("音效")]
    [SerializeField] private AudioClip Attack1AudioClip;

    [Header("招式2")]
    [SerializeField] private SpherePainter LandAttack2MaxMoveDistanceRadius;
    [SerializeField] private SpherePainter LandAttack2Radius;
    [Header("动画")]
    [SerializeField] private string Attack2StartAnimName = "Attack2Start";
    [SerializeField] private string Attack2WorkingAnimName = "Attack2Working";
    [SerializeField] private string Attack2EndAnimName = "Attack2End";
    [Header("音效")]
    [SerializeField] private AudioClip Attack2AudioClip;

    [Header("招式3")]
    [SerializeField] private SpherePainter LandAttack3MaxMoveDistanceRadius;
    [SerializeField] private SpherePainter LandAttack3Radius;
    [Header("动画")]
    [SerializeField] private string Attack3StartAnimName = "Attack3Start";
    [SerializeField] private string Attack3WorkingAnimName = "Attack3Working";
    [SerializeField] private string Attack3EndAnimName = "Attack3End";
    [Header("音效")]
    [SerializeField] private AudioClip Attack3AudioClip;

    [Header("特殊攻击")]

    [Header("阶段1")]
    [SerializeField] private SpherePainter SpecialAttackMaxMoveDistanceRadius;

    [SerializeField] private SpherePainter Part1_SpecialAttackRadius;

    [Header("动画")]
    [SerializeField] private string Part1_SpecialAttackAnimName = "SpecialAttackWorking";

    [Header("音效")]
    [SerializeField] private AudioClip Part1_SpecialAttackAudioClip;

    [Header("阶段2")]
    [SerializeField] private SpherePainter SpecialAttack2MaxMoveDistanceRadius;
    [SerializeField] private SpherePainter Part2_SpecialAttackRadius;
    [Header("动画")]
    [SerializeField] private string Part2_SpecialAttackAnimName = "SpecialAttackWorking2";

    [Header("音效")]
    [SerializeField] private AudioClip Part2_SpecialAttackAudioClip;

    [Header("特殊攻击结束动画")]
    [SerializeField] private string SpecialAttackEndAnimName = "SpecialAttackEnd";

    [Header("交互动画")]
    [SerializeField] private string InteractingReadyAnimName = "InteractingReady";

    [SerializeField] private string InteractingStartAnimName = "InteractingStart";


    private StateMachine<GirlStateType, InputEvent> GirlRootStateMachine= new();
    private StateMachine<GirlStateType,JumpUpType,InputEvent> JumpUpStateMachine = new();
    private StateMachine<GirlStateType, LandAttackType, InputEvent> LandAttackStateMachine = new();
    private StateMachine<GirlStateType, InteractingType, InputEvent> InteractingStateMachine = new();
    private StateMachine<GirlStateType, LandAttackType, InputEvent> SpecialAttackStateMachine = new();

    private bool isGround => girlCheckGround.GetIsGround;

    private InputManager inputManager => InputManager.Instance;
    private CharacterManager characterManager => CharacterManager.Instance;
    private BeatManager beatManager => BeatManager.Instance;

    private Vector3 moveDirection => inputManager.GetMoveDirection;

    private GirlStateType currentState => GirlRootStateMachine.ActiveStateName;

    private void Awake()
    {
        characterManager.SetCurrentCharacterData(girlData);

        Build_JumpUpMachine();
        Build_LandAttackMachine();
        Build_InteractingMachine();
        Build_SpecialAttackMachine();
        

        Build_RootMachine();
    }

    void OnEnable()
    {
        inputManager.AttackTapEvent += Tap_Attack;
        inputManager.AttackHoldEvent += Hold_Attack;
        inputManager.JumpEvent += Jump;
        inputManager.DodgeEvent += Dodge;

        LandAttackCollider.enabled = false;
        GirlRootStateMachine.Init();
    }


    void Update()
    {
        GirlRootStateMachine.OnLogic();
        //Debug.Log($"当前状态：{GirlRootStateMachine.GetActiveHierarchyPath()}");
    }

    void OnDisable()
    {
        GirlRootStateMachine.OnExit();

        if (inputManager)
        {
            inputManager.AttackTapEvent -= Tap_Attack;
            inputManager.AttackHoldEvent -= Hold_Attack;
            inputManager.JumpEvent -= Jump;
            inputManager.DodgeEvent -= Dodge;
        }
    }

    void OnDestroy()
    {
        GirlRootStateMachine.OnExit();

        if (inputManager)
        {
            inputManager.AttackTapEvent -= Tap_Attack;
            inputManager.AttackHoldEvent -= Hold_Attack;
            inputManager.JumpEvent -= Jump;
            inputManager.DodgeEvent -= Dodge;
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
        GirlRootStateMachine.AddState(GirlStateType.LandDodge, new LandDodge_Girl(animator, characterController, girlData, 
                                                                    Dodge_Front_AnimName, Dodge_Back_AnimName, 
                                                                    audioSource, F_DodgeAudioClip, A_DodgeAudioClip));
        GirlRootStateMachine.AddState(GirlStateType.InteractingMachine, InteractingStateMachine);

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

        //闪避状态切换
        GirlRootStateMachine.AddTransition(GirlStateType.LandDodge, GirlStateType.WaitingInput);

        //地面攻击状态切换
        GirlRootStateMachine.AddTransition(GirlStateType.LandAttackMachine, GirlStateType.WaitingInput, t => !girlData.GetIsLandAttacking);
        GirlRootStateMachine.AddTransition(GirlStateType.LandAttackMachine, GirlStateType.WaitingInput, 
                                            t => moveDirection != Vector3.zero && girlData.GetMoveInputWindow &&
                                            LandAttackStateMachine.ActiveStateName == LandAttackType.LandAttack1_End);
        GirlRootStateMachine.AddTransition(GirlStateType.LandAttackMachine, GirlStateType.WaitingInput, 
                                            t => moveDirection != Vector3.zero && girlData.GetMoveInputWindow &&
                                            LandAttackStateMachine.ActiveStateName == LandAttackType.LandAttack2_End, forceInstantly: true);
        GirlRootStateMachine.AddTransition(GirlStateType.LandAttackMachine, GirlStateType.WaitingInput, 
                                            t => moveDirection != Vector3.zero && girlData.GetMoveInputWindow &&
                                            LandAttackStateMachine.ActiveStateName == LandAttackType.LandAttack3_End, forceInstantly: true);
        
        //交互状态切换
        GirlRootStateMachine.AddTransition(GirlStateType.InteractingMachine, GirlStateType.WaitingInput, t => !girlData.IsInteracting);

        //任意状态切换
        GirlRootStateMachine.AddTransitionFromAny(GirlStateType.InteractingMachine, t => girlData.IsInteracting);
        GirlRootStateMachine.AddTriggerTransitionFromAny(InputEvent.Dodge, GirlStateType.LandDodge,
                                            t => isGround && currentState != GirlStateType.LandDodge && !girlData.IsInteracting, forceInstantly: true);
        GirlRootStateMachine.AddTriggerTransitionFromAny(InputEvent.Jump, GirlStateType.JumpUpMachine, 
                                            t => girlData.CanJump && currentState!= GirlStateType.JumpUpMachine && !girlData.IsInteracting, forceInstantly: true);
        GirlRootStateMachine.AddTriggerTransitionFromAny(InputEvent.AttackTap, GirlStateType.LandAttackMachine, 
                                            t => isGround && currentState != GirlStateType.LandAttackMachine && !girlData.IsInteracting, forceInstantly: true);
        GirlRootStateMachine.AddTransitionFromAny(GirlStateType.LandAttackMachine, t=> isGround && 
                                                                                currentState != GirlStateType.LandAttackMachine &&
                                                                                girlData.GetAttackTapInputWindow&&
                                                                                inputManager.AttackExpire && !girlData.IsInteracting);
        
        
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
        LandAttackStateMachine.AddState(LandAttackType.LandAttack_Enter, new LandAttack_Enter(girlData));

        // 清空闪避值状态
        LandAttackStateMachine.AddState(LandAttackType.ClearDodgeCount, new ClearDodgeValue_LandAttack(girlData));

        // 地面攻击1状态
        LandAttackStateMachine.AddState(LandAttackType.LandAttack1_Start, new LandAttack_Start(girlData, characterController, animator, 
                                                                    Animator.StringToHash(Attack1StartAnimName), 
                                                                    LandAttack1Radius.GetRadius, LandAttack1MaxMoveDistanceRadius.GetRadius));
        LandAttackStateMachine.AddState(LandAttackType.LandAttack1_Attack, new LandAttack_Attack(LandAttackType.LandAttack1_Attack,
                                                                                                LandAttackType.LandAttack2_Start,
                                                                                                girlData, animator, 
                                                                                                Animator.StringToHash(Attack1WorkingAnimName), 
                                                                                                LandAttackCollider,
                                                                                                audioSource, Attack1AudioClip));
        LandAttackStateMachine.AddState(LandAttackType.LandAttack1_End, new LandAttack_End(girlData, animator, 
                                                                    Animator.StringToHash(Attack1EndAnimName), 1f));
    
        // 地面攻击2状态
        LandAttackStateMachine.AddState(LandAttackType.LandAttack2_Start, new LandAttack_Start(girlData, characterController, animator, 
                                                                    Animator.StringToHash(Attack2StartAnimName), 
                                                                    LandAttack2Radius.GetRadius, LandAttack2MaxMoveDistanceRadius.GetRadius));
        LandAttackStateMachine.AddState(LandAttackType.LandAttack2_Attack, new LandAttack_Attack(LandAttackType.LandAttack2_Attack,
                                                                            LandAttackType.LandAttack_JBeatCheck,
                                                                            girlData, animator, 
                                                                            Animator.StringToHash(Attack2WorkingAnimName), 
                                                                            LandAttackCollider,
                                                                            audioSource, Attack2AudioClip));
        LandAttackStateMachine.AddState(LandAttackType.LandAttack2_End, new LandAttack_End(girlData, animator, 
                                                                    Animator.StringToHash(Attack2EndAnimName), 1f));

        // J节拍地面攻击检测状态
        LandAttackStateMachine.AddState(LandAttackType.LandAttack_JBeatCheck, new BeatDetection_LandAttack(animator, girlData, 
                                                                                                            BeatCheckType.JBeatCheck));

        // 结算表演值状态
        LandAttackStateMachine.AddState(LandAttackType.SetShowValue, new SettleShowValue_LandAttack(girlData));

        // 地面攻击3状态
        LandAttackStateMachine.AddState(LandAttackType.LandAttack3_Start, new LandAttack_Start(girlData, characterController, animator, 
                                                                    Animator.StringToHash(Attack3StartAnimName), 
                                                                    LandAttack3Radius.GetRadius, LandAttack3MaxMoveDistanceRadius.GetRadius));
        LandAttackStateMachine.AddState(LandAttackType.LandAttack3_Attack, new LandAttack_Attack(LandAttackType.LandAttack3_Attack,
                                                                                                LandAttackType.LandAttack1_Start,
                                                                                                girlData, animator, 
                                                                                                Animator.StringToHash(Attack3WorkingAnimName), 
                                                                                                LandAttackCollider,
                                                                                                audioSource, Attack3AudioClip));
        LandAttackStateMachine.AddState(LandAttackType.LandAttack3_End, new LandAttack_End(girlData, animator, 
                                                                    Animator.StringToHash(Attack3EndAnimName), 1f));     

        //进入转换
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack_Enter, LandAttackType.ClearDodgeCount, 
                                                t => girlData.NextLandAttackType == LandAttackType.LandAttack1_Start);
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack_Enter, LandAttackType.LandAttack2_Start, 
                                                t => girlData.NextLandAttackType == LandAttackType.LandAttack2_Start);
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack_Enter, LandAttackType.LandAttack_JBeatCheck, 
                                                t => girlData.NextLandAttackType == LandAttackType.LandAttack_JBeatCheck);

        // 清空闪避值转换
            LandAttackStateMachine.AddTransition(LandAttackType.ClearDodgeCount, LandAttackType.LandAttack1_Start);

        //地面攻击1
            // 起手 -> 攻击
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack1_Start, LandAttackType.LandAttack1_Attack);
            // 攻击1 -> 攻击2
            LandAttackStateMachine.AddTriggerTransition(InputEvent.AttackTap, LandAttackType.LandAttack1_Attack, 
                                                        LandAttackType.LandAttack2_Start, forceInstantly: true);
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack1_Attack, LandAttackType.LandAttack2_Start, 
                            t => inputManager.AttackExpire && girlData.GetAttackTapInputWindow, forceInstantly: true);
            //攻击 -> 结束
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack1_Attack, LandAttackType.LandAttack1_End);

            // 结束 -> 攻击2
            LandAttackStateMachine.AddTriggerTransition(InputEvent.AttackTap, LandAttackType.LandAttack1_End, 
                                                    LandAttackType.LandAttack2_Start, forceInstantly: true);
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack1_End, LandAttackType.LandAttack2_Start, 
                            t => inputManager.AttackExpire && girlData.GetAttackTapInputWindow, forceInstantly: true);

        //地面攻击2
            // 起手 -> 攻击2
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack2_Start, LandAttackType.LandAttack2_Attack);

            // 攻击2 -> j节拍检测
            LandAttackStateMachine.AddTriggerTransition(InputEvent.AttackTap, LandAttackType.LandAttack2_Attack, 
                                                        LandAttackType.LandAttack_JBeatCheck, 
                                                        forceInstantly: true);
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack2_Attack, LandAttackType.LandAttack_JBeatCheck,
                                                t => inputManager.AttackExpire && girlData.GetAttackTapInputWindow, forceInstantly: true);
            //攻击 -> 结束
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack2_Attack, LandAttackType.LandAttack2_End);

            // 结束 -> j节拍检测
            LandAttackStateMachine.AddTriggerTransition(InputEvent.AttackTap, LandAttackType.LandAttack2_End, 
                                                        LandAttackType.LandAttack_JBeatCheck, 
                                                        forceInstantly: true);
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack2_End, LandAttackType.LandAttack_JBeatCheck,
                                                t => inputManager.AttackExpire && girlData.GetAttackTapInputWindow, forceInstantly: true);

        // J节拍检测
            // j节拍检测 -> 结算表演值
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack_JBeatCheck, LandAttackType.SetShowValue,
                                                t => beatManager.JBeatCheckResult == BeatResult.Good);
            // 结节拍检测 -> 招式1
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack_JBeatCheck, LandAttackType.ClearDodgeCount,
                                                t => beatManager.JBeatCheckResult == BeatResult.Miss);
        
        // 结算表演值
            // 结算表演值 -> 招式3
            LandAttackStateMachine.AddTransition(LandAttackType.SetShowValue, LandAttackType.LandAttack3_Start);
        
        //地面攻击3
            // 起手 -> 攻击
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack3_Start, LandAttackType.LandAttack3_Attack);

            //攻击3 -> 清空闪避值
            LandAttackStateMachine.AddTriggerTransition(InputEvent.AttackTap, LandAttackType.LandAttack3_Attack, 
                                                        LandAttackType.ClearDodgeCount, forceInstantly: true);
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack3_Attack, LandAttackType.ClearDodgeCount, 
                            t => inputManager.AttackExpire && girlData.GetAttackTapInputWindow, forceInstantly: true);
            //攻击3 -> 结束
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack3_Attack, LandAttackType.LandAttack3_End);

            // 结束 -> 清空闪避值
            LandAttackStateMachine.AddTriggerTransition(InputEvent.AttackTap, LandAttackType.LandAttack3_End, 
                                                        LandAttackType.ClearDodgeCount, forceInstantly: true);
            LandAttackStateMachine.AddTransition(LandAttackType.LandAttack3_End, LandAttackType.ClearDodgeCount, 
                            t => inputManager.AttackExpire && girlData.GetAttackTapInputWindow, forceInstantly: true);


        LandAttackStateMachine.SetStartState(LandAttackType.LandAttack_Enter);
    }

    private void Build_SpecialAttackMachine()
    {
        
    }

    private void Build_InteractingMachine()
    {
        InteractingStateMachine.AddState(InteractingType.InteractingReady, new InteractingReady(animator, InteractingReadyAnimName));
        InteractingStateMachine.AddState(InteractingType.InteractingStart, new InteractingStarts(animator, InteractingStartAnimName));

        InteractingStateMachine.AddTransition(InteractingType.InteractingReady, InteractingType.InteractingStart, t => beatManager.CurrentBeatResult != BeatResult.none);
        
        InteractingStateMachine.AddTransition(InteractingType.InteractingStart, InteractingType.InteractingReady);

        InteractingStateMachine.SetStartState(InteractingType.InteractingReady);
    }

    #region 输入事件

    /// <summary>
    /// 点击攻击键
    /// </summary>
    void Tap_Attack()
    {
        if(!girlData.GetAttackTapInputWindow) return;

        GirlRootStateMachine.Trigger(InputEvent.AttackTap);
    }

    /// <summary>
    /// 长按攻击键
    /// </summary>
    void Hold_Attack()
    {
        if(!girlData.GetAttackHoldInputWindow) return;

        GirlRootStateMachine.Trigger(InputEvent.AttackHold);
    }

    /// <summary>
    /// 点击跳跃键
    /// </summary>
    void Jump()
    {
        if(!girlData.GetJumpInputWindow) return;

        GirlRootStateMachine.Trigger(InputEvent.Jump);
    }

    /// <summary>
    /// 点击闪避键
    /// </summary>
    void Dodge()
    {
        if(!girlData.GetDodgeInputWindow) return;

        GirlRootStateMachine.Trigger(InputEvent.Dodge);
    }

    #endregion

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

using System.Collections;
using UnityEngine;

public class LandDodge_Girl : CharacterState<GirlStateType>
{
    private Animator animator;
    private int currentDodgeAnimHash =0;
    private int Dodge_Front_Hash;
    private int Dodge_Back_Hash;

    private CharacterController character;
    private Girl_Data girlData;
    private GirlController characterController;
    private AudioClip F_DodgeAudioClip;
    private AudioClip A_DodgeAudioClip;

    /// <summary>
    /// 闪避方向，基于输入获取，进入状态时确定，并在状态内保持不变
    /// </summary>
    private Vector3 dodgeDirection;

    /// <summary>
    /// 闪避距离
    /// </summary>
    private float dodgeDistance => girlData.GetDodgeDistance;

    /// <summary>
    /// 闪避速度
    /// </summary>
    private float dodgeSpeed;

    private bool AniFinsh = false;
    public LandDodge_Girl(Animator animator, 
                        CharacterController character,
                        Girl_Data girlData,
                        string dodgeFrontAnimName, 
                        string dodgeBackAnimName,
                        GirlController characterController,
                        AudioClip F_DodgeAudioClip,
                        AudioClip A_DodgeAudioClip):base(needsExitTime: true, canExit: (state) => ((LandDodge_Girl)state).AniFinsh)
    {
        this.animator = animator;
        this.Dodge_Front_Hash = Animator.StringToHash(dodgeFrontAnimName);
        this.Dodge_Back_Hash = Animator.StringToHash(dodgeBackAnimName);

        this.character = character;
        this.girlData = girlData;

        this.characterController = characterController;
        this.F_DodgeAudioClip = F_DodgeAudioClip;
        this.A_DodgeAudioClip = A_DodgeAudioClip;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        AniFinsh = false;

        girlData.SetDodgeInputWindow(false);
        girlData.SetAttackTapInputWindow(false);
        girlData.SetJumpInputWindow(false);
        girlData.SetDodgeValue(girlData.GetDodgeValue + 1); // 进入闪避状态时增加闪避值

        dodgeDirection = inputManager.GetMoveDirection;
        dodgeSpeed = 0f;

        if (dodgeDirection != Vector3.zero)
        {
            character.transform.rotation = Quaternion.LookRotation(dodgeDirection);
            animator.Play(Dodge_Front_Hash);
            currentDodgeAnimHash = Dodge_Front_Hash;
        }
        else
        {
            dodgeDirection = -character.transform.forward; // 如果没有输入方向，默认向后闪避
            animator.Play(Dodge_Back_Hash);
            currentDodgeAnimHash = Dodge_Back_Hash;
        }
        coroutineManager.Run("LandDodge_Girl", DodgeCoroutine());

        // 播放闪避音效
        if(girlData.GetCurrentLandAttackType == LandAttackType.LandAttack2_Attack)
        {
            characterController.PlayAttackAudio(A_DodgeAudioClip);
        }
        else if(girlData.GetCurrentLandAttackType == LandAttackType.LandAttack1_Attack)
        {
            characterController.PlayAttackAudio(F_DodgeAudioClip);
        }
    }

    public override void OnLogic()
    {
        base.OnLogic();
        if(dodgeSpeed <= 0f) return; // 避免除以零或负数导致的异常

        
        character.Move(dodgeDirection.normalized * dodgeDistance * Time.deltaTime);
    }

    public override void OnExit()
    {
        base.OnExit();
        AniFinsh = false;

        girlData?.SetDodgeInputWindow(true);
        girlData?.SetAttackTapInputWindow(true);
        girlData?.SetJumpInputWindow(true);

        coroutineManager?.Stop("LandDodge_Girl");
    }

    IEnumerator DodgeCoroutine()
    {
        yield return null;
        float animationLength = AnimatorTool.GetRealAnimationLength_FullPath(animator, currentDodgeAnimHash);
        dodgeSpeed = dodgeDistance / animationLength;
        yield return new WaitForSeconds(animationLength);
        AniFinsh = true;
        fsm.StateCanExit();
    }
}

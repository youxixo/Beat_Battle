using System.Collections;
using UnityEngine;

public class LandAttack_Attack : CharacterState<LandAttackType>
{
    private LandAttackType currentLandAttackType;
    private LandAttackType nextLandAttackType;
    private Animator animator;
    private int AttackAnimationHash;
    private Girl_Data girlData;
    private bool animationFinished = false;
    private Collider landAttackCollider;
    private GirlController characterController;
    private AudioClip attackAudioClip;

    public LandAttack_Attack(LandAttackType currentLandAttackType, 
                            LandAttackType nextLandAttackType,
                            Girl_Data girl_Data,
                            Animator animator, 
                            int attackAnimationHash, 
                            Collider landAttackCollider,
                            GirlController characterController,
                            AudioClip attackAudioClip
                            ):base(needsExitTime: true,
                                    canExit: state => ((LandAttack_Attack)state).animationFinished)
    {
        this.currentLandAttackType = currentLandAttackType;
        this.nextLandAttackType = nextLandAttackType;
        this.girlData = girl_Data;
        this.animator = animator;
        this.AttackAnimationHash = attackAnimationHash;
        this.landAttackCollider = landAttackCollider;
        this.characterController = characterController;
        this.attackAudioClip = attackAudioClip;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        girlData.SetAttackTapInputWindow(false);
        girlData.SetMoveInputWindow(false);
        girlData.SetCurrentLandAttackType(currentLandAttackType);
        girlData.NextLandAttackType = nextLandAttackType;
        girlData.SetIsLandAttacking(true);

        inputManager.AttackExpire = false; // 重置攻击输入保质期
        
        
        if(beatManager.IsBGMInBeat)
        {
            girlData.CurrentShowValue ++;
        }

        landAttackCollider.enabled = true;

        animationFinished = false;
        
        animator.Play(AttackAnimationHash);
        coroutineManager.Run("LandAttack_Attack_Girl", AttackCoroutine());

        characterController.PlayAttackAudio(attackAudioClip);
    }

    public override void OnExit()
    {
        base.OnExit();
        coroutineManager?.Stop("LandAttack_Attack_Girl");
        
        animationFinished = false;
        landAttackCollider.enabled = false;

        if(girlData)
        {
            girlData.SetAttackTapInputWindow(true);
            girlData.SetMoveInputWindow(true);
        }

        if(inputManager)
        {
            inputManager.AttackExpire = false;
        }
    }

    IEnumerator AttackCoroutine()
    {
        yield return null;
        float animationLength = AnimatorTool.GetRealAnimationLength_FullPath(animator, AttackAnimationHash);
        yield return new WaitForSeconds(animationLength);

        animationFinished = true;
        fsm.StateCanExit();
    }

}

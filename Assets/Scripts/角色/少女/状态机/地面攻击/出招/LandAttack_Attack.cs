using System.Collections;
using UnityEngine;

public class LandAttack_Attack : CharacterState<LandAttackType>
{
    private LandAttackType currentLandAttackType;
    private Animator animator;
    private int AttackAnimationHash;
    private Girl_Data girlData;
    private bool animationFinished = false;
    private Collider landAttackCollider;

    private bool NeedBeatCheck;
    public LandAttack_Attack(LandAttackType currentLandAttackType,Girl_Data girl_Data,Animator animator, int attackAnimationHash, Collider landAttackCollider, bool needBeatCheck = false):base(needsExitTime: true,
                                                                      canExit: state => ((LandAttack_Attack)state).animationFinished)
    {
        this.currentLandAttackType = currentLandAttackType;
        this.girlData = girl_Data;
        this.animator = animator;
        this.AttackAnimationHash = attackAnimationHash;
        this.landAttackCollider = landAttackCollider;
        this.NeedBeatCheck = needBeatCheck;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        inputManager.SetAttackInputWindow(false);
        inputManager.AttackExpire = false; // 重置攻击输入保质期
        inputManager.SetMoveInputWindow(false);
        
        girlData.SetCurrentLandAttackType(currentLandAttackType);

        if (NeedBeatCheck)
        {
            beatManager.CurrentBeatResult = BeatResult.none; // 重置节拍结果
            beatManager.SetCharacterReadyForBeatCheck(false); // 重置角色节拍检测准备状态
        }

        girlData.SetIsLandAttacking(true);
        landAttackCollider.enabled = true;

        animationFinished = false;
        
        animator.Play(AttackAnimationHash);
        coroutineManager.Run("LandAttack_Attack_Girl", AttackCoroutine());
    }

    public override void OnExit()
    {
        base.OnExit();
        coroutineManager?.Stop("LandAttack_Attack_Girl");
        
        animationFinished = false;
        landAttackCollider.enabled = false;

        if(inputManager)
        {
            inputManager.SetAttackInputWindow(true);
            inputManager.SetMoveInputWindow(true);
            inputManager.AttackExpire = false;
        }

        if(beatManager && NeedBeatCheck)
        {
            beatManager.StopBeatCheckAction?.Invoke();
        }
    }

    IEnumerator AttackCoroutine()
    {
        yield return null;
        float animationLength = AnimatorTool.GetRealAnimationLength_FullPath(animator, AttackAnimationHash);
        yield return new WaitForSeconds(animationLength);

        if (NeedBeatCheck)
        {
            beatManager.SetCharacterReadyForBeatCheck(true);
            beatManager.StartBeatCheck();

            yield return new WaitUntil(() => beatManager.CurrentBeatResult != BeatResult.none);
            yield return null;
        }

        animationFinished = true;
        fsm.StateCanExit();
    }

}

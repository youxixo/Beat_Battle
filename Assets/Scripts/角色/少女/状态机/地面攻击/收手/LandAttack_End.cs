using System.Collections;
using UnityEngine;

public class LandAttack_End : CharacterState<LandAttackType>
{
    private Animator animator;
    private int AttackAnimationHash;
    private float waitingTime = 0.1f; // 等待时间，确保动画状态切换完成
    private Girl_Data girlData;

    public LandAttack_End(Girl_Data girl_Data, Animator animator, int attackAnimationHash, float waitingTime):base(needsExitTime: true,
                                                                      canExit: state => AnimatorTool.IsAnimationFinished_FullPath(animator, attackAnimationHash))
    {
        this.animator = animator;
        this.AttackAnimationHash = attackAnimationHash;
        this.waitingTime = waitingTime;
        this.girlData = girl_Data;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        inputManager.SetMoveInputWindow(true);
        girlData.SetIsLandAttacking(true);

        animator.Play(AttackAnimationHash);
        coroutineManager.Run("LandAttack_End_Girl", AttackCoroutine());
    }

    public override void OnExit()
    {
        base.OnExit();
        coroutineManager?.Stop("LandAttack_End_Girl");
        girlData.SetIsLandAttacking(false);
    }

    IEnumerator AttackCoroutine()
    {
        yield return null;
        float animationLength = AnimatorTool.GetRealAnimationLength_FullPath(animator, AttackAnimationHash);
        yield return new WaitForSeconds(animationLength);
        yield return new WaitForSeconds(waitingTime);
        girlData.SetIsLandAttacking(false);
    }
}

using System.Collections;
using UnityEngine;

public class LandAttack_Attack : CharacterState<LandAttackType>
{
    private Animator animator;
    private int AttackAnimationHash;
    private Girl_Data girlData;
    private bool animationFinished = false;
    public LandAttack_Attack(Girl_Data girl_Data,Animator animator, int attackAnimationHash):base(needsExitTime: true,
                                                                      canExit: state => ((LandAttack_Attack)state).animationFinished)
    {
        this.girlData = girl_Data;
        this.animator = animator;
        this.AttackAnimationHash = attackAnimationHash;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        girlData.SetIsLandAttacking(true);

        animationFinished = false;
        inputManager.SetAttackInputWindow(false);
        animator.Play(AttackAnimationHash);
        coroutineManager.Run("LandAttack_Attack_Girl", AttackCoroutine());
    }

    public override void OnExit()
    {
        base.OnExit();
        coroutineManager?.Stop("LandAttack_Attack_Girl");
        inputManager.SetAttackInputWindow(true);
        animationFinished = false;
    }

    IEnumerator AttackCoroutine()
    {
        yield return null;
        float animationLength = AnimatorTool.GetRealAnimationLength_FullPath(animator, AttackAnimationHash);
        yield return new WaitForSeconds(animationLength);
        Debug.Log("攻击1动画结束");
        animationFinished = true;
        fsm.StateCanExit();
    }

}

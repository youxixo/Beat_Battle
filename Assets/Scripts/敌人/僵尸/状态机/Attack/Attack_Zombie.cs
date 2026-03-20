using System.Collections;
using UnityEngine;

public class Attack_Zombie : BaseState<ZombieType>
{
    private Animator animator;
    private int AttackHash;
    private Collider[] AttackBoxCollider;
    private bool AnimationFinished = false;
    private ZombieDate zombieDate;

    private DownTimer AttackCooldownTimer;

    private CoroutineManager coroutineManager => CoroutineManager.Instance;
    public Attack_Zombie(Animator animator, 
                        string attackAnimName, 
                        Collider[] attackBoxCollider, 
                        DownTimer attackCooldownTimer, 
                        ZombieDate zombieDate) : base(needsExitTime: true, 
                                                canExit: State =>((Attack_Zombie)State).AnimationFinished)
    {
        this.animator = animator;
        AttackHash = Animator.StringToHash(attackAnimName);
        AttackBoxCollider = attackBoxCollider;
        AttackCooldownTimer = attackCooldownTimer;
        this.zombieDate = zombieDate;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    
        foreach (var collider in AttackBoxCollider)
        {
            collider.enabled = false;
        }

        AnimationFinished = false;

        animator.Play(AttackHash);
        coroutineManager.Run("Zombie_Attack" + animator.gameObject.GetInstanceID(), Zombie_Attack());
    }

    public override void OnExit()
    {
        base.OnExit();

        foreach (var collider in AttackBoxCollider)
        {
            collider.enabled = true;
        }

        coroutineManager?.Stop("Zombie_Attack" + animator.gameObject.GetInstanceID());
        AttackCooldownTimer?.ResetTimer(zombieDate.AttackCooldown);
        AttackCooldownTimer?.StartTimer();
    }

    IEnumerator Zombie_Attack()
    {
        yield return null; // 等待一帧，确保动画状态已切换

        float animationLength = AnimatorTool.GetRealAnimationLength(animator, AttackHash);
        yield return new WaitForSeconds(animationLength);
        AnimationFinished = true;
        fsm.StateCanExit();
    }
}

using UnityEngine;

public class Idle_Zombie : BaseState<ZombieType>
{
    private Animator animator;
    private int IdleHash;

    public Idle_Zombie(Animator animator, string idleAnimName) : base()
    {
        this.animator = animator;
        IdleHash = Animator.StringToHash(idleAnimName);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        animator.Play(IdleHash);
    }
}

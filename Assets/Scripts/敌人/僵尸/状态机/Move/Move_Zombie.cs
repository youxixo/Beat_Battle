using UnityEngine;

public class Move_Zombie : BaseState<ZombieType>
{
    private Animator animator;
    private int MoveHash;
    private ZombieDate zombieDate;

    private Transform girlData => CharacterManager.Instance.GetCurrentCharacterData.transform;

    public Move_Zombie(Animator animator, string moveAnimName, ZombieDate zombieDate) : base(needsExitTime: false)
    {
        this.animator = animator;
        MoveHash = Animator.StringToHash(moveAnimName);
        this.zombieDate = zombieDate;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    
        animator.Play(MoveHash);
    }

    public override void OnLogic()
    {
        base.OnLogic();

        zombieDate.transform.LookAt(girlData);

        zombieDate.transform.position += zombieDate.transform.forward * zombieDate.CurrentMoveSpeed * Time.deltaTime;
    }
}

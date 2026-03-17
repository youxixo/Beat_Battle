using UnityEngine;

public class Run_Girl : CharacterState<GirlStateType>
{
    private Animator animator;
    private int runHash;
    private CharacterController character;
    private float moveSpeed;

    public Run_Girl(CharacterController character,float moveSpeed, Animator animator, int runHash) : base()
    {
        this.character = character;
        this.moveSpeed = moveSpeed;
        this.animator = animator;
        this.runHash = runHash;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        animator.Play(runHash);
        Debug.Log("进入奔跑状态");
    }

    public override void OnLogic()
    {
        base.OnLogic();

        Vector2 inputDir = inputManager.GetMoveDirection;
        Vector3 moveDir = new Vector3(inputDir.x, 0, inputDir.y);
        
        character.Move(moveDir * moveSpeed * Time.deltaTime);
        character.transform.rotation = Quaternion.LookRotation(moveDir);
    }
}

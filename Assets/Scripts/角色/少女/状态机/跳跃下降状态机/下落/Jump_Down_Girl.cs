using UnityEngine;

public class Jump_Down_Girl: CharacterState<GirlStateType>
{
    private Animator animator;
    
    private int jumpDownHash;

    private Girl_Data girlData;

    private CharacterController character;

    private Vector2 moveInput => InputManager.Instance.GetMoveDirection;

	public Jump_Down_Girl(Girl_Data girlData, Animator animator, int jumpDownHash, CharacterController character): base()
    {
        this.girlData = girlData;
        this.animator = animator;
        this.jumpDownHash = jumpDownHash;
        this.character = character;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        animator.Play(jumpDownHash);
    }

    public override void OnLogic()
    {
        base.OnLogic();

        float velocityY = girlData.GetCurrentYVelocity;

        velocityY += Physics.gravity.y * Time.deltaTime;


        girlData.SetCurrentYVelocity(velocityY);

        Vector3 velocity = new Vector3(
            moveInput.x * girlData.GetMoveSpeed,
            velocityY,
            moveInput.y * girlData.GetMoveSpeed
        );

        character.Move(velocity * Time.deltaTime);
    }
}

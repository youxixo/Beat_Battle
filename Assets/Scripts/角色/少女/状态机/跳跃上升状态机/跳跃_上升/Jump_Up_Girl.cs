using UnityEngine;

public class Jump_Up_Girl : CharacterState<JumpUpType>
{
    private CharacterController character;
    private Girl_Data girlData;
    private Vector2 moveInput => InputManager.Instance.GetMoveDirection;
    private Animator animator;
    private int jumpUpHash;
    public Jump_Up_Girl(Girl_Data girlData, CharacterController character, Animator animator, int jumpUpHash):base()
    {
        this.girlData = girlData;
        this.character = character;
        this.animator = animator;
        this.jumpUpHash = jumpUpHash;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        girlData.SetCurrentYVelocity(girlData.GetJumpForce);

        animator.Play(jumpUpHash);
    }

    public override void OnLogic()
    {
        base.OnLogic();
        
        // 重力
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

    public override void OnExit()
    {
        base.OnExit();

        girlData.SetCurrentYVelocity(0);
    }
}

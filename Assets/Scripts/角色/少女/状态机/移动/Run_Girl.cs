using UnityEngine;

public class Run_Girl : CharacterState<GirlStateType>
{
    private Animator animator;
    private int runHash;
    private CharacterController character;
    private Girl_Data girl_Data;
    private float moveSpeed;
    private float rotateSpeed;

    private Vector3 lastMoveDirection;

    public Run_Girl(CharacterController character,Girl_Data girl_Data, Animator animator, int runHash) : base()
    {
        this.character = character;
        this.girl_Data = girl_Data;
        this.animator = animator;
        this.runHash = runHash;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        cameraManager.SwitchCamera(CameraType.DefaultCamera);

        moveSpeed = girl_Data.GetMoveSpeed;
        rotateSpeed = girl_Data.GetRotateSpeed;

        animator.Play(runHash);
    }

    public override void OnLogic()
    {
        base.OnLogic();

        Vector3 rawInput = inputManager.GetMoveDirection; 

        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        Transform camTransform = mainCam.transform;

        Vector3 camForward = Vector3.Scale(camTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(camTransform.right, new Vector3(1, 0, 1)).normalized;

        Vector3 moveDir = (camForward * rawInput.z + camRight * rawInput.x).normalized;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            character.Move(moveDir * moveSpeed * Time.deltaTime);

            Quaternion targetRotation;

            // ⭐ 核心修复
            if (rawInput.z < 0)
            {
                targetRotation = Quaternion.LookRotation(-camForward);
            }
            else
            {
                targetRotation = Quaternion.LookRotation(moveDir);
            }

            character.transform.rotation = Quaternion.Slerp(
                character.transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime);

            lastMoveDirection = moveDir;
        }
    }



    public override void OnExit()
    {
        base.OnExit();
    }
}

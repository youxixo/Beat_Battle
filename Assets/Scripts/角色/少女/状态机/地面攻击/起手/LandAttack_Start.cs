using UnityEngine;

public class LandAttack_Start : CharacterState<LandAttackType>
{
    private LandAttackType NextLandAttackType;
    private Animator animator;
    private int AttackAnimationHash;

    private Girl_Data girlData;
    private CharacterController characterController;

    private float AttackRadius;
    private float MoveDistanceToEnemy = 0f;
    private float MaxMoveDistance = 2f;

    private Vector3 moveDirection;
    private float rotationSpeed = 15f;

    private float moveSpeed;

    private bool animationFinished = false;

    public LandAttack_Start(
        Girl_Data girlData,
        CharacterController characterController,
        Animator animator,
        int attackAnimationHash,
        LandAttackType nextLandAttackType,
        float attackRadius,
        float maxMoveDistance)
        : base(needsExitTime: true,
              canExit: state => ((LandAttack_Start)state).animationFinished)
    {
        this.girlData = girlData;
        this.characterController = characterController;
        this.animator = animator;
        this.AttackAnimationHash = attackAnimationHash;
        this.NextLandAttackType = nextLandAttackType;
        this.AttackRadius = attackRadius;
        this.MaxMoveDistance = maxMoveDistance;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        girlData.SetIsLandAttacking(true);
        
        inputManager.SetAttackInputWindow(false);
        inputManager.SetMoveInputWindow(false);
        inputManager.AttackExpire = false; // 重置攻击输入保质期

        MoveDistanceToEnemy = 0f;
        girlData.NextLandAttackType = NextLandAttackType;

        if (enemyManager.currentTargetEnemy)
        {
            Vector3 dir = enemyManager.currentTargetEnemy.transform.position - characterController.transform.position;
            dir.y = 0;

            float distance = dir.magnitude;

            moveDirection = dir.normalized;
            characterController.transform.rotation = Quaternion.LookRotation(moveDirection);

            if (distance > AttackRadius)
            {
                MoveDistanceToEnemy = Mathf.Min(distance - AttackRadius - 0.2f, MaxMoveDistance);
            }
        }
        else
        {
            moveDirection = characterController.transform.forward;
        }

        animator.Play(AttackAnimationHash);

        float animLength = AnimatorTool.GetRealAnimationLength(animator, AttackAnimationHash);
        moveSpeed = MoveDistanceToEnemy / Mathf.Max(animLength - 0.05f, 0.01f);

        cameraManager.SwitchCamera(CameraType.BattleCamera);
    }

    public override void OnLogic()
    {
        base.OnLogic();

        // ✅ 每帧判断动画是否结束
        animationFinished = AnimatorTool.IsAnimationFinished_FullPath(animator, AttackAnimationHash);

        // ✅ 移动
        if (MoveDistanceToEnemy > 0f && !animationFinished)
        {
            float deltaMove = moveSpeed * Time.deltaTime;

            characterController.Move(moveDirection * deltaMove);

            MoveDistanceToEnemy -= deltaMove;
        }

        // ✅ 锁敌旋转（每帧）
        if (enemyManager.currentTargetEnemy)
        {
            Vector3 dir = enemyManager.currentTargetEnemy.transform.position - characterController.transform.position;
            dir.y = 0;

            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
                characterController.transform.rotation = Quaternion.Slerp(
                    characterController.transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime);
            }
        }

        // ✅ 退出条件（核心）
        if (animationFinished)
        {
            fsm.StateCanExit();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        girlData.SetIsLandAttacking(false);

        if(inputManager)
        {
            inputManager.SetMoveInputWindow(true);
            inputManager.SetAttackInputWindow(true);
            inputManager.AttackExpire = false; // 重置攻击输入保质期
        }
        
    }
}

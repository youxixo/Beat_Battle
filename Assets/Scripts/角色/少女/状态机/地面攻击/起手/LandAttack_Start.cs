using System.Collections;
using UnityEngine;

public class LandAttack_Start : CharacterState<LandAttackType>
{
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
        float attackRadius,
        float maxMoveDistance)
        : base(needsExitTime: true,
              canExit: state => ((LandAttack_Start)state).animationFinished)
    {
        this.girlData = girlData;
        this.characterController = characterController;
        this.animator = animator;
        this.AttackAnimationHash = attackAnimationHash;
        this.AttackRadius = attackRadius;
        this.MaxMoveDistance = maxMoveDistance;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        girlData.SetIsLandAttacking(true);

        girlData.SetAttackTapInputWindow(false);
        girlData.SetMoveInputWindow(false);

        inputManager.AttackExpire = false;

        MoveDistanceToEnemy = 0f;

        // ===== 计算方向 =====
        if (enemyManager.currentTargetEnemy)
        {
            Vector3 dir = enemyManager.currentTargetEnemy.transform.position - characterController.transform.position;
            dir.y = 0;

            float distance = dir.magnitude;

            moveDirection = dir.normalized;

            if (moveDirection.sqrMagnitude > 0.001f)
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

        coroutineManager.Run("LandAttack_Start_Init_" + AttackAnimationHash, InitAfterAnimStart());

        if(enemyManager.currentTargetEnemy)
        {
            cameraManager.SwitchCamera(CameraType.BattleCamera);
        }
    }

    public override void OnLogic()
    {
        base.OnLogic();

        // =========================
        // ✅ 动画结束判断
        // =========================
        animationFinished = AnimatorTool.IsAnimationFinished_FullPath(animator, AttackAnimationHash);

        // =========================
        // ✅ 移动（带剩余距离保护）
        // =========================
        if (MoveDistanceToEnemy > 0f && !animationFinished)
        {
            float deltaMove = moveSpeed * Time.deltaTime;

            // 防止越界（最后一帧抖动）
            deltaMove = Mathf.Min(deltaMove, MoveDistanceToEnemy);

            characterController.Move(moveDirection * deltaMove);

            MoveDistanceToEnemy -= deltaMove;
        }

        // =========================
        // ✅ 锁敌旋转（平滑）
        // =========================
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

        // =========================
        // ✅ 退出
        // =========================
        if (animationFinished)
        {
            fsm.StateCanExit();
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        girlData.SetIsLandAttacking(false);

        // ⭐ 必须重置动画速度！
        animator.speed = 1f;

        if (girlData)
        {
            girlData.SetMoveInputWindow(true);
            girlData.SetAttackTapInputWindow(true);
        }

        inputManager.AttackExpire = false;
    }

    IEnumerator InitAfterAnimStart()
    {
        yield return null;

        float animLength = AnimatorTool.GetRealAnimationLength(animator, AttackAnimationHash);

        float targetSpeed = girlData.GetLandAttackMoveSpeed;

        if (MoveDistanceToEnemy > 0.01f)
        {
            float moveTime = MoveDistanceToEnemy / targetSpeed;

            float animSpeed = animLength / Mathf.Max(moveTime, 0.01f);

            animSpeed = Mathf.Clamp(animSpeed, 0.8f, 1.5f);

            animator.speed = animSpeed;

            moveSpeed = targetSpeed;
        }
        else
        {
            animator.speed = 1f;
            moveSpeed = 0f;
        }
    }
}
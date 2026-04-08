using System.Collections;
using UnityEngine;

public class LandAttack_Attack : CharacterState<LandAttackType>
{
    private LandAttackType currentLandAttackType;
    private LandAttackType nextLandAttackType;
    private Animator animator;
    private int AttackAnimationHash;
    private Girl_Data girlData;
    private bool animationFinished = false;
    private Collider landAttackCollider;
    private GirlController characterController;
    private AudioClip attackAudioClip;

    // ===== 新增：追击参数 =====
    private float AttackRadius;
    private float MoveDistanceToEnemy = 0f;
    private float MaxMoveDistance = 2f;
    private Vector3 moveDirection;
    private float rotationSpeed = 15f;
    private float moveSpeed;

    public LandAttack_Attack(
        LandAttackType currentLandAttackType,
        LandAttackType nextLandAttackType,
        Girl_Data girl_Data,
        Animator animator,
        int attackAnimationHash,
        Collider landAttackCollider,
        GirlController characterController,
        AudioClip attackAudioClip,
        float attackRadius,
        float maxMoveDistance)
        : base(needsExitTime: true,
               canExit: state => ((LandAttack_Attack)state).animationFinished)
    {
        this.currentLandAttackType = currentLandAttackType;
        this.nextLandAttackType = nextLandAttackType;
        this.girlData = girl_Data;
        this.animator = animator;
        this.AttackAnimationHash = attackAnimationHash;
        this.landAttackCollider = landAttackCollider;
        this.characterController = characterController;
        this.attackAudioClip = attackAudioClip;

        this.AttackRadius = attackRadius;
        this.MaxMoveDistance = maxMoveDistance;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        girlData.SetAttackTapInputWindow(false);
        girlData.SetMoveInputWindow(false);
        girlData.SetCurrentLandAttackType(currentLandAttackType);
        girlData.NextLandAttackType = nextLandAttackType;
        girlData.SetIsLandAttacking(true);

        inputManager.AttackExpire = false;

        if (beatManager.IsBGMInBeat)
        {
            girlData.CurrentShowValue++;
        }

        landAttackCollider.enabled = true;
        animationFinished = false;

        // ===== 追击初始化 =====
        MoveDistanceToEnemy = 0f;

        if (enemyManager.currentTargetEnemy)
        {
            Vector3 dir = enemyManager.currentTargetEnemy.transform.position - characterController.transform.position;
            dir.y = 0f;

            float distance = dir.magnitude;
            moveDirection = dir.sqrMagnitude > 0.001f ? dir.normalized : characterController.transform.forward;

            if (moveDirection.sqrMagnitude > 0.001f)
            {
                characterController.transform.rotation = Quaternion.LookRotation(moveDirection);
            }

            // 只追到攻击半径附近
            if (distance > AttackRadius)
            {
                MoveDistanceToEnemy = Mathf.Min(distance - AttackRadius - 0.2f, MaxMoveDistance);
            }
        }
        else
        {
            moveDirection = characterController.transform.forward;
        }

        // 追击速度：你也可以换成更适合 Attack 段的速度
        moveSpeed = girlData.GetLandAttackMoveSpeed;

        animator.Play(AttackAnimationHash);
        coroutineManager.Run("LandAttack_Attack_Girl", AttackCoroutine());

        characterController.PlayAttackAudio(attackAudioClip);
    }

    public override void OnLogic()
    {
        base.OnLogic();

        // ===== 追击时持续锁敌转身 =====
        if (enemyManager.currentTargetEnemy)
        {
            Vector3 dir = enemyManager.currentTargetEnemy.transform.position - characterController.transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
            {
                moveDirection = dir.normalized;

                Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                characterController.transform.rotation = Quaternion.Slerp(
                    characterController.transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );
            }
        }

        // ===== 追击位移 =====
        if (MoveDistanceToEnemy > 0f && !animationFinished)
        {
            float deltaMove = moveSpeed * Time.deltaTime;
            deltaMove = Mathf.Min(deltaMove, MoveDistanceToEnemy);

            // 如果 GirlController 里有 Move 方法，就用这个
            characterController.transform.position += moveDirection * deltaMove;

            MoveDistanceToEnemy -= deltaMove;
        }

        if (animationFinished)
        {
            fsm.StateCanExit();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        coroutineManager?.Stop("LandAttack_Attack_Girl");

        animationFinished = false;
        landAttackCollider.enabled = false;

        if (girlData)
        {
            girlData.SetAttackTapInputWindow(true);
            girlData.SetMoveInputWindow(true);
            girlData.SetIsLandAttacking(false);
        }

        if (inputManager)
        {
            inputManager.AttackExpire = false;
        }

        animator.speed = 1f;
    }

    IEnumerator AttackCoroutine()
    {
        yield return null;

        float animationLength = AnimatorTool.GetRealAnimationLength_FullPath(animator, AttackAnimationHash);
        yield return new WaitForSeconds(animationLength);

        animationFinished = true;
        fsm.StateCanExit();
    }
}
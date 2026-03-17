using System.Collections;
using UnityEngine;

public class Girl_Data : MonoBehaviour
{
    private CoroutineManager coroutineManager => CoroutineManager.Instance;
    [Header("移动属性")]
    #region 移动属性
    [SerializeField] private float moveSpeed = 5f;
    public float GetMoveSpeed => moveSpeed;

    [SerializeField] private float RotateSpeed = 720f;
    public float GetRotateSpeed => RotateSpeed;
    #endregion

    [Header("跳跃属性")]
    #region 跳跃属性
    [SerializeField] private float jumpForce = 50f;
    public float GetJumpForce => jumpForce;
    /// <summary>
    /// 当前Y轴速度
    /// </summary>
    [SerializeField] private float currentYVelocity = 0f;
    public float GetCurrentYVelocity => currentYVelocity;
    /// <summary>
    /// 设置当前Y轴速度
    /// </summary>
    /// <param name="velocity"></param>
    public void SetCurrentYVelocity(float velocity)
    {
        currentYVelocity = velocity;
    }
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField]private int currentJumpCount = 0;

    public int GetCurrentJumpCount => currentJumpCount;
    public void SetCurrentJumpCount(int count)
    {
        currentJumpCount = count;
    }

    /// <summary>
    /// 能否执行跳跃
    /// </summary>
    public bool CanJump
    {
        get=> currentJumpCount < maxJumpCount;
    }
    #endregion

    #region 地面攻击属性
    [SerializeField] private bool IsLandAttacking = false;
    /// <summary>
    /// 获取当前是否处于地面攻击状态
    /// </summary>
    public bool GetIsLandAttacking => IsLandAttacking;
    /// <summary>
    /// 设置当前是否处于地面攻击状态，通常由状态机调用，在进入地面攻击状态时设置为true，退出时设置为false。
    /// </summary>
    public void SetIsLandAttacking(bool value)
    {
        IsLandAttacking = value;
    }

    /// <summary>
    /// 地面攻击招式的持续时间，超过这个时间后会自动重置为初始招式
    /// </summary>
    [SerializeField] private float landAttackEx = 2f;

    [SerializeField] private LandAttackType nextLandAttackType = LandAttackType.LandAttack1_Start;
    public LandAttackType NextLandAttackType
    {
        set
        {
            if(value != nextLandAttackType)
            {
                coroutineManager.Run("LandAttackExpiration_Girl", LandAttackExpirationCoroutine());
                nextLandAttackType = value;
            }
        }
        get => nextLandAttackType;
    }

    IEnumerator LandAttackExpirationCoroutine()
    {
        yield return new WaitForSeconds(landAttackEx);
        nextLandAttackType = LandAttackType.LandAttack1_Start;
    }

    #endregion
}

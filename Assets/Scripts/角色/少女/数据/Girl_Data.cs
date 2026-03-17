using UnityEngine;

public class Girl_Data : MonoBehaviour
{
    [Header("移动属性")]
    #region 移动属性
    [SerializeField] private float moveSpeed = 5f;
    public float GetMoveSpeed => moveSpeed;
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
}

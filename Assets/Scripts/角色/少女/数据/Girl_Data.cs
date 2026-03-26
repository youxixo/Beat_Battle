using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Girl_Data : MonoBehaviour
{
    private CoroutineManager coroutineManager => CoroutineManager.Instance;

    [Header("受击数")]
    [SerializeField, ChineseLabel("最大受击数")] private int maxHitCount = 3;
    public int GetMaxHitCount => maxHitCount;
    [SerializeField, ChineseLabel("当前受击数")] private int currentHitCount = 0;
    public int CurrentHitCount
    {
        get => currentHitCount;
        set
        {
            currentHitCount = Mathf.Clamp(value, 0, maxHitCount);
            HitCountChangeAction?.Invoke(currentHitCount);

            if (currentHitCount >= maxHitCount)
            {
                // 触发眩晕状态
                IsDizziness = true;
            }
        }
    }

    [ChineseLabel("受击数改变事件")] public UnityEvent<float> HitCountChangeAction;


    #region 眩晕属性
    [Header("眩晕时间")]
    [SerializeField, ChineseLabel("眩晕时间")] private float dizzinessTime = 1f;
    /// <summary>
    /// 获取眩晕时间
    /// </summary>
    public float GetDizzinessTime => dizzinessTime;

    [SerializeField, ChineseLabel("是否处于眩晕状态")] private bool isDizziness = false;
    /// <summary>
    /// 获取是否处于眩晕状态    
    /// </summary>
    public bool IsDizziness
    {
        get => isDizziness;
        set => isDizziness = value;
    }


    #endregion

    [Header("移动属性")]
    #region 移动属性
    [SerializeField, ChineseLabel("移动速度")] private float moveSpeed = 5f;
    public float GetMoveSpeed => moveSpeed;

    [SerializeField, ChineseLabel("旋转速度")] private float RotateSpeed = 720f;
    public float GetRotateSpeed => RotateSpeed;
    #endregion

    [Header("跳跃属性")]
    #region 跳跃属性
    [SerializeField, ChineseLabel("跳跃力大小")] private float jumpForce = 50f;
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
    [SerializeField, ChineseLabel("跳跃上限")] private int maxJumpCount = 2;
    [SerializeField, ChineseLabel("当前跳跃次数")] private int currentJumpCount = 0;

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

    #region 闪避属性
    [Header("闪避属性")]
    [SerializeField, ChineseLabel("闪避距离")] private float dodgeDistance = 5f;
    public float GetDodgeDistance => dodgeDistance;

    [Header("闪避次数：决定表演值增加量")]
    /// <summary>
    /// 闪避次数，决定表演值增加量
    /// </summary>
    [SerializeField, ChineseLabel("一套攻击内的闪避次数")] private int dodgeValue = 0;

    /// <summary>
    /// 获取当前闪避次数
    /// </summary>
    public int GetDodgeValue => dodgeValue;
    public void SetDodgeValue(int value)
    {
        dodgeValue = value;
    }
    #endregion

    #region 地面攻击属性
    [Header("地面攻击属性")]
    [SerializeField] private float landAttackMoveSpeed = 3f;
    public float GetLandAttackMoveSpeed => landAttackMoveSpeed;
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

    [Space(10)]
    [Header("地面攻击招式的保质期，如果在这个时间内没有接续招式，则重置为初始招式")]
    /// <summary>
    /// 地面攻击招式的持续时间，超过这个时间后会自动重置为初始招式
    /// </summary>
    [SerializeField, ChineseLabel("地面攻击招式的有效期")] private float landAttackEx = 2f;


    private LandAttackType CurrentLandAttackType = LandAttackType.LandAttack1_Start;
    /// <summary>
    /// 获取当前地面攻击的招式类型
    /// </summary>
    public LandAttackType GetCurrentLandAttackType => CurrentLandAttackType;
    public void SetCurrentLandAttackType(LandAttackType type)
    {
        CurrentLandAttackType = type;
    }
    
    [SerializeField, ChineseLabel("下一个地面攻击招式类型")] private LandAttackType nextLandAttackType = LandAttackType.LandAttack1_Start;
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

    #region 交互事件
    [SerializeField, ChineseLabel("是否正在交互状态")] private bool isInteracting = false;

    /// <summary>
    /// 是否要进行交互状态
    /// </summary>
    public bool IsInteracting => isInteracting;

    public void SetIsInteracting(bool value)
    {
        isInteracting = value;
    }
    #endregion

    #region 输入框口
    [Header("输入窗口")]
    /// <summary>
    /// 短按攻击输入窗口
    /// </summary>
    [SerializeField, ChineseLabel("短按攻击输入窗口")] private bool attackTapInputWindow = true;

    /// <summary>
    /// 获取短按攻击输入窗口状态
    /// </summary>
    public bool GetAttackTapInputWindow => attackTapInputWindow;
    public void SetAttackTapInputWindow(bool value)
    {
        attackTapInputWindow = value;
    }

    /// <summary>
    /// 长按攻击输入窗口
    /// </summary>
    [SerializeField, ChineseLabel("长按攻击输入窗口")] private bool attackHoldInputWindow = true;
    /// <summary>
    /// 获取长按攻击输入窗口状态
    /// </summary>
    public bool GetAttackHoldInputWindow => attackHoldInputWindow;
    public void SetAttackHoldInputWindow(bool value)
    {
        attackHoldInputWindow = value;
    }

    /// <summary>
    /// 跳跃输入窗口
    /// </summary>
    [SerializeField, ChineseLabel("跳跃输入窗口")] private bool jumpInputWindow = true;
    /// <summary>
    /// 获取跳跃输入窗口状态
    /// </summary>
    public bool GetJumpInputWindow => jumpInputWindow;
    public void SetJumpInputWindow(bool value)
    {
        jumpInputWindow = value;
    }

    /// <summary>
    /// 闪避输入窗口
    /// </summary>
    [SerializeField, ChineseLabel("闪避输入窗口")] private bool dodgeInputWindow = true;
    /// <summary>
    /// 获取闪避输入窗口状态
    /// </summary>
    public bool GetDodgeInputWindow => dodgeInputWindow;
    public void SetDodgeInputWindow(bool value)
    {
        dodgeInputWindow = value;
    }

    /// 移动输入窗口
    [SerializeField, ChineseLabel("移动输入窗口")] private bool moveInputWindow = true;
    /// <summary>
    /// 获取移动输入窗口状态
    /// </summary>
    public bool GetMoveInputWindow => moveInputWindow;
    public void SetMoveInputWindow(bool value)
    {
        moveInputWindow = value;
    }
    #endregion

    #region 表现值
    [Header("表现值")]
    [SerializeField, ChineseLabel("最大表现值")] private int MaxShowValue = 5;
    public int GetMaxShowValue => MaxShowValue;
    [SerializeField, ChineseLabel("当前表现值")] private int currentShowValue = 0;
    [ChineseLabel("当表现值改变时触发")]public UnityEvent<float> ShowValueChangeAction;
    public int CurrentShowValue
    {
        get => currentShowValue;
        set
        {
            currentShowValue = Mathf.Clamp(value, 0, MaxShowValue);
            ShowValueChangeAction?.Invoke(currentShowValue);
        }
    }
    #endregion
}
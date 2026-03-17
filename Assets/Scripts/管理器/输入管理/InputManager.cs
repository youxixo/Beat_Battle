using System;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    #region WASD
    [SerializeField]private Vector3 MoveDirection = Vector3.zero;
    
    /// <summary>
    /// 获取玩家的移动方向，使用WASD键控制。
    /// </summary>
    public Vector3 GetMoveDirection
    {
        get { return MoveDirection.normalized; }
    }

    /// <summary>
    /// 设置玩家的移动方向，通常由输入系统调用，传入WASD键的输入值。
    /// </summary>
    /// <param name="moveDirection">玩家的移动方向，通常是WASD键的输入值。</param>
    public void SetMoveDirection(Vector2 moveDirection)
    {
        MoveDirection = new Vector3(moveDirection.x, 0, moveDirection.y);
    }

    #endregion

    #region 攻击键
    /// <summary>
    /// 攻击短按事件，通常由输入系统调用，当玩家按下攻击键时触发，执行短按攻击逻辑。
    /// </summary>
    public Action AttackTapEvent;

    /// <summary>
    /// 攻击长按事件，通常由输入系统调用，当玩家持续按下攻击键时触发，执行长按攻击逻辑。
    /// </summary>
    public Action AttackHoldEvent;

    /// <summary>
    /// 攻击键输入窗口
    /// </summary>
    [SerializeField]private bool AttackInputWindow = true;

    /// <summary>
    /// 获取攻击键输入窗口状态
    /// </summary>
    public bool GetAttackInputWindow
    {
        get { return AttackInputWindow; }
    }

    /// <summary>
    /// 设置攻击键输入窗口，通常由输入系统调用，当玩家按下攻击键时打开窗口，释放时关闭窗口。
    /// </summary>
    public void SetAttackInputWindow(bool isOpen)
    {
        AttackInputWindow = isOpen;
    }
    #endregion

    #region 跳跃键
    public Action JumpEvent;

    /// <summary>
    /// 跳跃键输入窗口
    /// </summary>
    [SerializeField]private bool JumpInputWindow = true;

    /// <summary>
    /// 获取跳跃键输入窗口状态
    /// </summary>
    public bool GetJumpInputWindow
    {
        get { return JumpInputWindow; }
    }

    /// <summary>
    /// 设置跳跃键输入窗口，通常由输入系统调用，当玩家按下跳跃键时打开窗口，释放时关闭窗口。
    /// </summary>
    public void SetJumpInputWindow(bool isOpen)
    {
        JumpInputWindow = isOpen;
    }

    #endregion

}

using System;
using System.Collections;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private CoroutineManager coroutineManager => CoroutineManager.Instance;
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

    ///<summary>
    /// 攻击键是否在保质期内
    /// </summary>
    private bool attackExpire = false;

    /// <summary>
    /// 攻击输入保质期，通常由输入系统调用，当玩家按下攻击键时设置。
    /// </summary>
    public bool AttackExpire
    {
        set
        {
            if(value)
            {
                coroutineManager.Run("AttackExpire", AttackExpireCoroutine());
            }
            else
            {
                coroutineManager.Stop("AttackExpire");
            }
            attackExpire = value;
        }
        get { return attackExpire; }
    }

    IEnumerator AttackExpireCoroutine()
    {
        yield return new WaitForSeconds(0.5f); // 攻击输入保质期为0.5秒
        attackExpire = false;
    }

    #endregion

    #region 跳跃键
    public Action JumpEvent;

    #endregion

    #region 闪避键
    /// <summary>
    /// 闪避键输入事件，通常由输入系统调用，当玩家按下闪避键时触发，执行闪避逻辑。
    /// </summary>
    public Action DodgeEvent;
    #endregion

    #region 交互键
    /// <summary>
    /// 交互键输入事件，游戏对象自主订阅，当玩家按下交互键时触发，执行交互逻辑。
    /// </summary>
    public Action InteractEvent;
    #endregion

}

using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private InputManager inputManager => InputManager.Instance;

    /// <summary>
    /// 移动输入回调函数，当玩家按下或释放WASD键时调用，更新InputManager中的移动方向。
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 moveDirection = context.ReadValue<Vector2>();
        
            if (moveDirection.magnitude > 0.1f)
            {
                inputManager.SetMoveDirection(moveDirection);
            }
            else
            {
                inputManager.SetMoveDirection(Vector2.zero);
            }
        }
        else if (context.canceled)
        {
            inputManager.SetMoveDirection(Vector2.zero);
        }
    }

    /// <summary>
    /// 短按攻击
    /// <para>当玩家按下攻击键时调用，触发InputManager中的攻击事件。</para>
    /// </summary>
    /// <param name="context"></param>
    public void OnAttackTap(InputAction.CallbackContext context)
    {
        bool attackInputWindow = inputManager.GetAttackInputWindow;
        
        if (context.performed)
        {
            if(attackInputWindow)
            {
                inputManager.AttackTapEvent?.Invoke();
            }
            inputManager.AttackExpire = true; // 设置攻击输入保质期
        }
    }

    /// <summary>
    /// 长按攻击
    /// <para>当玩家持续按下攻击键时调用，触发InputManager中的攻击事件。</para>
    /// </summary>
    /// <param name="context"></param>
    public void OnAttackHold(InputAction.CallbackContext context)
    {
        bool attackInputWindow = inputManager.GetAttackInputWindow;

        if (context.performed && attackInputWindow)
        {
            inputManager.AttackHoldEvent?.Invoke();
        }
    }

    /// <summary>
    /// 跳跃输入回调函数，当玩家按下或释放跳跃键时调用，更新InputManager中的跳跃状态。
    /// </summary>
    /// <param name="context"></param>
    public void OnJump(InputAction.CallbackContext context)
    {
        bool jumpInputWindow = inputManager.GetJumpInputWindow;

        if (context.performed && jumpInputWindow)
        {
            inputManager.JumpEvent?.Invoke();
        }
    }
    
    public void OnDodge(InputAction.CallbackContext context)
    {
        bool dodgeInputWindow = inputManager.GetDodgeInputWindow;

        if (context.performed && dodgeInputWindow)
        {
            inputManager.DodgeEvent?.Invoke();
        }
    }
}

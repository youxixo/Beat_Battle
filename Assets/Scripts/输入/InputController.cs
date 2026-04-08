using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private InputManager inputManager => InputManager.Instance;
    private CameraManager cameraManager => CameraManager.Instance;
    private CoroutineManager coroutineManager => CoroutineManager.Instance;

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
        
        if (context.performed)
        {
            inputManager.AttackTapEvent?.Invoke();

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

        if (context.performed)
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
        if (context.performed)
        {
            inputManager.JumpEvent?.Invoke();
        }
    }
    
    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            inputManager.DodgeEvent?.Invoke();
        }
    }

    /// <summary>
    /// 交互输入回调函数，当玩家按下交互键时调用，
    /// </summary>
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            inputManager.InteractEvent?.Invoke();
        }
    }

    /// <summary>
    /// 相机顺时针旋转
    /// </summary>
    public void OnCameraRotateClockwise(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            coroutineManager.Run("CameraRotate", cameraManager.RotateCameraClockwise());
        }
        else if (context.canceled)
        {
            coroutineManager.Stop("CameraRotate");
        }
    }

    /// <summary>
    /// 相机逆时针旋转
    /// </summary>
    public void OnCameraRotateCounterClockwise(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            coroutineManager.Run("CameraRotate", cameraManager.RotateCameraCounterClockwise());
        }
        else if (context.canceled)
        {
            coroutineManager.Stop("CameraRotate");
        }
    }

    /// <summary>
    /// 鼠标控制相机旋转
    /// </summary>
    public void OnMouseControlCameraRotation(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            coroutineManager.Run("CameraRotate", cameraManager.MouseControlCameraRotation());
        }
        else if (context.canceled)
        {
            coroutineManager.Stop("CameraRotate");
        }
    }
}

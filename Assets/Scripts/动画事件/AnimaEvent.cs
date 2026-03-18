using UnityEngine;

public class AnimaEvent : MonoBehaviour
{
    private InputManager inputManager => InputManager.Instance;

    /// <summary>
    /// 攻击输入窗口控制，1代表开启攻击输入窗口，0代表关闭攻击输入窗口
    /// </summary>
    public void SetAttackInputWindow(int canAttack)
    {
        if (canAttack == 1)
        {
            inputManager?.SetAttackInputWindow(true);
        }
        else
        {
            inputManager?.SetAttackInputWindow(false);
        }
    }

    /// <summary>
    /// 跳跃输入窗口控制，1代表开启跳跃输入窗口，0代表关闭跳跃输入窗口
    /// </summary>
    public void SetJumpInputWindow(int canJump)
    {
        if (canJump == 1)
        {
            inputManager?.SetJumpInputWindow(true);
        }
        else
        {
            inputManager?.SetJumpInputWindow(false);
        }
    }
}

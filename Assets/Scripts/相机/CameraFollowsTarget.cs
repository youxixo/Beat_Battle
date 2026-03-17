using UnityEngine;

public class CameraFollowsTarget : MonoBehaviour
{
    public Transform target; // 目标对象


    private InputManager inputManager => InputManager.Instance;
    void LateUpdate()
    {
        if (target != null)
        {
            // 将摄像机的位置设置为目标对象的位置
            transform.position = target.position;
        }
    }
}

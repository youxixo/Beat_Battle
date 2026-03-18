using UnityEngine;

public static class Tool
{
    /// <summary>
    /// 判断目标位置相对于当前角色是左侧还是右侧
    /// </summary>
    /// <param name="targetPos">目标位置</param>
    /// <param name="currentPos">当前角色位置</param>
    /// <param name="forward">当前角色前方向</param>
    /// <returns>是否在左侧</returns>

    public static bool IsLeft(Vector3 targetPos, Vector3 currentPos, Vector3 forward)
    {
        Vector3 toTarget = targetPos - currentPos;
        float dot = Vector3.Dot(forward, toTarget);
        if (dot == 0)
        {
            return false; // 目标在正前方或正后方
        }
        else if (dot > 0)
        {
            return Vector3.Cross(forward, toTarget).y > 0; // 目标在左侧
        }
        else
        {
            return Vector3.Cross(forward, toTarget).y < 0; // 目标在右侧
        }
    }
}

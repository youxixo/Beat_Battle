using UnityEngine;

public static class AnimatorTool
{
    /// <summary>
    /// 检测动画是否播完
    /// </summary>
    public static bool IsAnimationFinished(Animator animator, int animationHashName, int layerIndex = 0)
    {
        if (animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash == animationHashName)
        {
            return animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime >= 0.98f;
        }
        return false;
    }

    /// <summary>
    /// 检测动画是否播完（使用fullPathHash）
    /// </summary>
    public static bool IsAnimationFinished_FullPath(Animator animator, int fullPathHash, int layerIndex = 0)
    {
        if (animator == null) return false;

        var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        if (stateInfo.fullPathHash != fullPathHash)
            return false;

        if (animator.IsInTransition(layerIndex))
            return false;

        if (stateInfo.loop)
            return false;

        return stateInfo.normalizedTime >= 0.98f;
    }

    /// <summary>
    /// 获取动画真实播放时长（考虑速度）
    /// </summary>
    public static float GetRealAnimationLength(Animator animator, int animationHashName, int layerIndex = 0)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        if (stateInfo.shortNameHash != animationHashName)
            return 0f;

        float baseLength = stateInfo.length;

        // Animator 全局速度
        float animatorSpeed = animator.speed;

        // 当前状态速度（包含 Animator Controller 设置）
        float stateSpeed = stateInfo.speed;

        // speedMultiplier（有时会用）
        float speedMultiplier = stateInfo.speedMultiplier;

        float totalSpeed = animatorSpeed * stateSpeed * speedMultiplier;

        if (totalSpeed <= 0f)
            return baseLength; // 防止除0

        return baseLength / totalSpeed;
    }

    public static float GetRealAnimationLength_FullPath(Animator animator, int fullPathHash, int layerIndex = 0)
    {
        if (animator == null) return 0f;

        var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        // ❗没切进去直接返回0（调用方要自己处理）
        if (stateInfo.fullPathHash != fullPathHash)
            return 0f;

        float baseLength = stateInfo.length;

        float totalSpeed = animator.speed * stateInfo.speed * stateInfo.speedMultiplier;

        if (totalSpeed <= 0f)
            return baseLength;

        return baseLength / totalSpeed;
    }
}

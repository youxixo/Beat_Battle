using UnityEngine;

public class UpTimer : MonoBehaviour
{
    /// <summary>
    /// 定时器是否正在运行
    /// </summary>
    protected bool M_isRunning = false;

    /// <summary>
    /// 定时器已经运行时间(秒)
    /// </summary>
    protected float M_elapsedTime = 0f;

    /// <summary>
    /// 暂停时刻记录的时间(秒)
    /// </summary>
    protected float M_pauseTimestamp = 0f;

    /// <summary>
    /// 计时器的速率(秒)
    /// </summary>
    protected float M_timeScale = 1f;

    /// <summary>
    /// 该计时器是否在运行
    /// </summary>
    public bool IsRunning => M_isRunning;

    /// <summary>
    /// 获取计时器已经运行时间(秒)
    /// </summary>
    public float Elapsed => M_elapsedTime;

    /// <summary>
    /// 启动 / 继续 计时
    /// </summary>
    public virtual void StartTimer()
    {
        if (M_isRunning)
            return;
        M_isRunning = true;

        // 如果之前有暂停记录时间，则继续从暂停时间开始计时
        if (M_pauseTimestamp > 0f)
        {
            M_elapsedTime = M_pauseTimestamp;
            M_pauseTimestamp = 0f;
        }
    }

    /// <summary>
    /// 暂停计时
    /// </summary>
    public virtual void PauseTimer()
    {
        if (!M_isRunning)
            return;
        M_isRunning = false;
        M_pauseTimestamp = M_elapsedTime;
    }

    /// <summary>
    /// 重置并暂停计时器
    /// </summary>
    public virtual void ResetTimer()
    {
        M_isRunning = false;
        M_elapsedTime = 0f;
        M_pauseTimestamp = 0f;
    }
    
    /// <summary>
    /// 更新计时器
    /// </summary>
    public void UpdateTimer(float deltaTime)
    {
        if (M_isRunning)
        {
            M_elapsedTime += deltaTime * M_timeScale;
        }
    }
}

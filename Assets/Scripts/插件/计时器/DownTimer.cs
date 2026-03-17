
/// <summary>
/// 倒计时计时器
/// </summary>
public class DownTimer
{
    /// <summary>
    /// 定时器是否正在运行
    /// </summary>
    protected bool M_isRunning = false;

    /// <summary>
    /// 定时器剩余时间(秒)
    /// </summary>
    protected float M_remainingTime = 0f;

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
    /// 倒计时是否结束
    /// </summary>
    public bool IsComplete() => M_remainingTime <= 0f;

    /// <summary>
    /// 获取计时器剩余时间(秒)
    /// </summary>
    public float GetRemainingTime() => M_remainingTime;

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
            M_remainingTime = M_pauseTimestamp;
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
        M_pauseTimestamp = M_remainingTime;
    }

    /// <summary>
    /// 重置计时器
    /// <para>如果未指定倒计时时间，则重置为0</para>
    /// </summary>
    public virtual void ResetTimer(float duration = 0f)
    {
        M_isRunning = false;
        M_remainingTime = duration;
        M_pauseTimestamp = 0f;
    }

    /// <summary>
    /// 设置倒计时开始时间
    /// </summary>
    /// <param name="duration">倒计时开始时间(秒)</param>
    public virtual void SetDuration(float duration)
    {
        M_remainingTime = duration;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (M_isRunning)
        {
            if (M_remainingTime > 0f)
            {
                M_remainingTime -= deltaTime * M_timeScale;
            }
            else
            {
                M_remainingTime = 0f;
                M_isRunning = false;
            }
        }
    }
}

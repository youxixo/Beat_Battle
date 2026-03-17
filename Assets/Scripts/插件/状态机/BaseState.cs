using System;
using UnityHFSM;

/// <summary>
/// 通用状态基类：支持可选的退出延迟 + 退出条件检查
/// 核心原则：退出流程由 OnExitRequest 统一控制，OnLogic 只负责倒计时
/// </summary>
public class BaseState<TStateId> : State<TStateId>
{
    private bool _isWaitingToExit = false;
    private readonly float _exitDelay;
    
    private readonly Action<State<TStateId>> _onEnter;
    private readonly Action<State<TStateId>> _onLogic;
    private readonly Action<State<TStateId>> _onExit;
    private readonly Func<State<TStateId>, bool> _canExit;

    public BaseState(
        bool needsExitTime = false,
        float exitDelay = 0.1f,
        bool isGhostState = false,
        Action<State<TStateId>> onEnter = null,
        Action<State<TStateId>> onLogic = null,
        Action<State<TStateId>> onExit = null,
        Func<State<TStateId>, bool> canExit = null
    ) : base(needsExitTime: needsExitTime, isGhostState: isGhostState)
    {
        _exitDelay = exitDelay;
        _onEnter = onEnter;
        _onLogic = onLogic;
        _onExit = onExit;
        _canExit = canExit;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _isWaitingToExit = false;
        timer.Reset();
        _onEnter?.Invoke(this);
    }

    public override void OnLogic()
    {
        base.OnLogic();
        _onLogic?.Invoke(this);

        // 唯一退出触发器：等待期结束 + 条件仍满足 → 退出
        if (_isWaitingToExit && timer.Elapsed >= _exitDelay)
        {
            // 安全起见，退出前再检查一次条件（防止等待期间条件变化）
            if (_canExit == null || _canExit(this))
            {
                fsm.StateCanExit();
            }
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        _onExit?.Invoke(this);
        _isWaitingToExit = false; // 防御性清理
    }

    /// <summary>
    /// 统一退出决策入口
    /// 流程：检查条件 → 满足则判断是否需要延迟 → 执行相应操作
    /// </summary>
    public override void OnExitRequest()
    {
        // 1. 检查退出条件（没有 canExit 委托则默认允许）
        bool conditionMet = (_canExit == null) || _canExit(this);
        
        if (!conditionMet)
        {
            // 条件不满足，拒绝退出，继续留在当前状态
            return;
        }

        // 2. 条件满足，处理延迟逻辑
        if (!needsExitTime)
        {
            // 不需要延迟 → 立即退出
            fsm.StateCanExit();
        }
        else
        {
            // 需要延迟 → 进入等待模式
            _isWaitingToExit = true;
            timer.Reset(); // ✅ 关键：必须重置计时器！
        }
    }
}
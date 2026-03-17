public enum GirlStateType
{
    /// <summary>
    /// 等待输入状态，角色处于等待玩家输入的状态
    /// </summary>
    WaitingInput,

    /// <summary>
    /// 奔跑状态
    /// </summary>

    Run,

    /// <summary>
    /// 待机状态
    /// </summary>
    Idle,

    /// <summary>
    /// 跳跃上升状态
    /// </summary>
    JumpUpMachine,

    /// <summary>
    /// 跳跃下降状态
    /// </summary>
    JumpDown,

    /// <summary>
    /// 跳跃落地状态
    /// </summary>
    JumpLand,

    /// <summary>
    /// 地面攻击状态
    /// </summary>
    LandAttackMachine,
}

public enum JumpUpType
{
    /// <summary>
    /// 跳跃进入状态
    /// </summary>
    JumpUp_Enter,
    /// <summary>
    /// 跳跃上升状态
    /// </summary>
    Jump_Up,
}

public enum LandAttackType
{
    LandAttack_Enter,
    /// <summary>
    /// 地面攻击招式1起手状态
    /// </summary>
    LandAttack1_Start,

    /// <summary>
    /// 地面攻击招式1攻击
    /// </summary>
    LandAttack1_Attack,

    /// <summary>
    /// 地面攻击招式1结束状态
    /// </summary>
    LandAttack1_End,

}



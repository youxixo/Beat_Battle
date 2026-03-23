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
    /// 地面闪避状态
    /// </summary>
    LandDodge,

    /// <summary>
    /// 地面攻击状态
    /// </summary>
    LandAttackMachine,

    /// <summary>
    /// 交互状态机
    /// </summary>
    InteractingMachine,

    /// <summary>
    /// 特殊攻击状态机
    /// </summary>
    SpecialAttackMachine,
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
    /// 清空闪避数量
    /// </summary>
    ClearDodgeCount,


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


    /// <summary>
    /// 地面攻击招式2起手状态
    /// </summary>
    LandAttack2_Start,
    /// <summary>
    /// 地面攻击招式2攻击状态
    /// </summary>
    LandAttack2_Attack,
    /// <summary>
    /// 地面攻击招式2结束状态
    /// </summary>
    LandAttack2_End,

    /// <summary>
    /// J节拍地面攻击检测状态
    /// </summary>
    LandAttack_JBeatCheck,

    /// <summary>
    /// 结算表演值
    /// </summary>
    SetShowValue,

    /// <summary>
    /// 地面攻击招式3起手状态
    /// </summary>
    LandAttack3_Start,
    /// <summary>
    /// 地面攻击招式3攻击状态
    /// </summary>
    LandAttack3_Attack,
    /// <summary>
    /// 地面攻击招式3结束状态
    /// </summary>
    LandAttack3_End,



    /// <summary>
    /// 特殊攻击进入状态
    /// </summary>
    SpecialAttack_Enter,

    /// <summary>
    /// 双节拍判定状态
    /// </summary>
    BothBeatCheck,

    /// <summary>
    /// 特殊攻击起手状态
    /// </summary>
    SpecialAttack_Part1,
    /// <summary>
    /// 特殊攻击攻击状态
    /// </summary>
    SpecialAttack_Part2,

    /// <summary>
    /// 特殊攻击结束状态
    /// </summary>
    SpecialAttack_End,
}

public enum InteractingType
{
    /// <summary>
    /// 交互准备状态
    /// </summary>
    InteractingReady,
    /// <summary>
    /// 交互开始状态
    /// </summary>
    InteractingStart,
}

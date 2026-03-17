using UnityEngine;

public enum CameraType
{
    /// <summary>
    /// 默认摄像机，跟随玩家移动
    /// </summary>
    DefaultCamera,

    /// <summary>
    /// 自由摄像机，允许玩家自由旋转
    /// </summary>
    FreeCamera,

    /// <summary>
    /// 战斗摄像机
    /// </summary>
    BattleCamera,
}
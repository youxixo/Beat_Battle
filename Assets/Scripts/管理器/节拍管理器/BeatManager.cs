using System;
using UnityEngine;

public enum BeatResult
{
    none,
    Good,
    Miss
}

public class BeatManager : Singleton<BeatManager>
{
    /// <summary>
    /// 开启节拍检测事件
    /// </summary>
    public Action StartBeatCheckAction;

    public void StartBeatCheck()
    {
        if(CharacterReadyForBeatCheck)
            StartBeatCheckAction?.Invoke();
    }

    /// <summary>
    /// 关闭节拍检测事件
    /// </summary>
    public Action StopBeatCheckAction;

    /// <summary>
    /// 检测节拍结果事件，参数为节拍结果
    /// </summary>
    public Action<BeatResult> BeatCheckResultAction;

    [SerializeField] private BeatResult currentBeatResult = BeatResult.none;
    public BeatResult CurrentBeatResult
    {
        get => currentBeatResult;
        set
        {
            currentBeatResult = value;
            if(currentBeatResult != BeatResult.none)
            {
                BeatCheckResultAction?.Invoke(currentBeatResult);
            }
        }
    }

    private bool characterReadyForBeatCheck = false;
    /// <summary>
    /// 当前角色准备好节拍检测
    /// </summary>
    public bool CharacterReadyForBeatCheck => characterReadyForBeatCheck;

    /// <summary>
    /// 设置当前角色是否准备好节拍检测，通常由角色控制器调用
    /// </summary>
    public void SetCharacterReadyForBeatCheck(bool value)
    {
        characterReadyForBeatCheck = value;
    }
}

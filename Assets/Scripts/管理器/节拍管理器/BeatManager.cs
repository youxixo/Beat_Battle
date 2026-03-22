using System;
using System.Collections;
using UnityEngine;

public enum BeatCheckType
{
    JBeatCheck,
    KBeatCheck,
    BothCheck,
}

public enum BeatResult
{
    none,
    Good,
    Miss
}

public class BeatManager : Singleton<BeatManager>
{
    private CoroutineManager coroutineManager => CoroutineManager.Instance;

    #region 节拍检测类型
    /// <summary>
    /// 当前节拍检测类型
    /// </summary>
    [SerializeField]private BeatCheckType currentBeatCheckType;
    public BeatCheckType CurrentBeatCheckType => currentBeatCheckType;
    #endregion

    #region 节拍检测结果
    /// <summary>
    /// 当前节拍检测结果
    /// <para> 无视检测类型，只管检测结果 </para>
    /// </summary>
    [SerializeField] private BeatResult currentBeatResult = BeatResult.none;
    public Action<BeatResult> BeatCheckResultAction;

     /// <summary>
    /// 当前节拍检测结果
    /// <para> 无视检测类型，只管检测结果 </para>
    /// </summary>
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

    #endregion

    #region J节拍检测和结果
    /// <summary>
    /// J 节拍测试结果
    /// </summary>
    [SerializeField] private BeatResult jBeatCheckResult = BeatResult.none;

    /// <summary>
    /// 开始 J节拍检测事件
    /// </summary>
    public Action JBeatStartCheckAction;

    /// <summary>
    /// J 节拍检测结果事件，参数为J节拍检测结果
    /// </summary>
    public Action<BeatResult> JBeatCheckResultAction;
    public BeatResult JBeatCheckResult
    {
        get => jBeatCheckResult;
        set
        {
            jBeatCheckResult = value;
            if(jBeatCheckResult != BeatResult.none)
            {
                JBeatCheckResultAction?.Invoke(jBeatCheckResult);
            }
            currentBeatResult = jBeatCheckResult;
        }
    }
    #endregion

    #region K节拍检测和结果
    /// <summary>
    /// K 节拍测试结果
    /// </summary>
    [SerializeField] private BeatResult kBeatCheckResult = BeatResult.none;

    /// <summary>
    /// 开始 K节拍检测事件
    /// </summary>
    public Action KBeatStartCheckAction;

    /// <summary>
    /// K 节拍检测结果事件，参数为K节拍检测结果
    /// </summary>
    public Action<BeatResult> KBeatCheckResultAction;
    public BeatResult KBeatCheckResult
    {
        get => kBeatCheckResult;
        set
        {
            kBeatCheckResult = value;
            if(kBeatCheckResult != BeatResult.none)
            {
                KBeatCheckResultAction?.Invoke(kBeatCheckResult);
            }
            currentBeatResult = kBeatCheckResult;
        }
    }
    #endregion

    #region 同时检测J和K节拍事件
    /// <summary>
    /// 同时开启J节拍和K节拍检测事件
    /// <para>参数为节拍检测时间间隔，单位为秒</para>
    /// </summary>
    public Action<BeatCheckType,float> BothBeatStartCheckAction;

    public Action<BeatResult> BothBeatCheckResultAction;

    #endregion
    /// <summary>
    /// 开启节拍检测事件
    /// </summary>
    public Action StartBeatCheckAction;

    #region 开始节拍检测
    /// <summary>
    /// 开始J或者K节拍检测事件，参数为节拍检测类型
    /// </summary>
    public void StartBeatCheck(BeatCheckType BeackTypr)
    {
        if(CharacterReadyForBeatCheck)
        {
            currentBeatResult = BeatResult.none;
            switch (BeackTypr)
            {
                case BeatCheckType.JBeatCheck:
                    currentBeatCheckType = BeatCheckType.JBeatCheck;
                    JBeatCheckResult = BeatResult.none;
                    JBeatStartCheckAction?.Invoke();
                    break;
                case BeatCheckType.KBeatCheck:
                    currentBeatCheckType = BeatCheckType.KBeatCheck;
                    KBeatCheckResult = BeatResult.none;
                    KBeatStartCheckAction?.Invoke();
                    break;
            }
        }
    }
    
    /// <summary>
    /// 同时开始J和K节拍检测事件
    /// </summary>
    /// <param name="FirstCheckType">先检测的节拍类型</param>
    /// <param name="interval">两个节拍检测的时间间隔，单位为 秒</param>
    public void StartBothBeatCheck(BeatCheckType FirstCheckType,float interval)
    {
        if(CharacterReadyForBeatCheck)
        {
            currentBeatCheckType = BeatCheckType.BothCheck;
            JBeatCheckResult = BeatResult.none;
            KBeatCheckResult = BeatResult.none;
            currentBeatResult = BeatResult.none;
            coroutineManager.Run("WaitJKBeatCheckResult", WaitJKBeatCheckResult());
            BothBeatStartCheckAction?.Invoke(FirstCheckType, interval);
        }
    }

    /// <summary>
    /// 等待JK节拍检测结果协程
    /// </summary>
    private IEnumerator WaitJKBeatCheckResult()
    {
        yield return new WaitUntil(() => JBeatCheckResult != BeatResult.none && KBeatCheckResult != BeatResult.none);
        if(JBeatCheckResult == BeatResult.Good && KBeatCheckResult == BeatResult.Good)
        {
            currentBeatResult = BeatResult.Good;
            BothBeatCheckResultAction?.Invoke(BeatResult.Good);
        }
        else
        {
            currentBeatResult = BeatResult.Miss;
            BothBeatCheckResultAction?.Invoke(BeatResult.Miss);
        }
    }

    #endregion



    /// <summary>
    /// 关闭节拍检测事件
    /// </summary>
    public Action StopBeatCheckAction;

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

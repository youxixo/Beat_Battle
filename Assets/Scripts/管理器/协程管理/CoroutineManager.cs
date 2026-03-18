using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 协程管理器，提供带ID的协程管理功能，允许通过ID
/// 启动、停止协程，并自动清理已完成的协程ID。适用于需要频繁启动和停止协程的场景，如角色状态管理等。
/// </summary>
public class CoroutineManager : Singleton<CoroutineManager>
{
   private Dictionary<string, Coroutine> coroutineDict = new();

   protected override void OnDestroy()
    {
        base.OnDestroy();
        StopAllManaged();
    }

    /// <summary>
    /// 启动协程（带ID）
    /// 如果ID已存在，会自动停止旧协程
    /// </summary>
    public void Run(string id, IEnumerator routine)
    {
        if (coroutineDict.ContainsKey(id))
        {
            Stop(id);
        }

        Coroutine coroutine = StartCoroutine(Wrapper(id, routine));
        coroutineDict[id] = coroutine;
    }

    /// <summary>
    /// 启动协程（不管理ID）
    /// </summary>
    public Coroutine Run(IEnumerator routine)
    {
        return Instance.StartCoroutine(routine);
    }

    /// <summary>
    /// 停止指定ID协程
    /// </summary>
    public void Stop(string id)
    {
        if (coroutineDict.TryGetValue(id, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
            coroutineDict.Remove(id);
        }
    }

    ///<summary>
    /// 检查指定ID的协程是否正在运行
    /// </summary>
    public bool IsRunning(string id)
    {
        return coroutineDict.ContainsKey(id);
    }

    /// <summary>
    /// 停止所有管理的协程
    /// </summary>
    public void StopAllManaged()
    {
        foreach (var pair in coroutineDict)
        {
            if (pair.Value != null)
            {
                StopCoroutine(pair.Value);
            }
        }

        coroutineDict.Clear();
    }

    /// <summary>
    /// 包装器：协程结束后自动移除ID
    /// </summary>
    private IEnumerator Wrapper(string id, IEnumerator routine)
    {
        yield return routine;

        if (coroutineDict.ContainsKey(id))
        {
            coroutineDict.Remove(id);
        }
    }
}

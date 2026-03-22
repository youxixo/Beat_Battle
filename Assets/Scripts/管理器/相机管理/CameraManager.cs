using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
   private CinemachineCamera CurrentCamera;
    ///<summary>
    /// 设置默认摄像机
    /// </summary>
    public void SetDefaultCamera(CinemachineCamera camera)
    {
        DefaultCamera = camera;
        if (CurrentCamera == null)
        {
            CurrentCamera = DefaultCamera;
            CurrentCamera.Priority = 20; // 确保默认摄像机优先级为20
        }
    }

    /// <summary>
    /// 默认摄像机
    /// </summary>
    [SerializeField] private CinemachineCamera DefaultCamera;

    /// <summary>
    /// 战斗相机（左）
    /// </summary>
    [SerializeField] private CinemachineCamera BattleCamera_Left;
    public void SetBattleCameraLeft(CinemachineCamera camera)
    {
        BattleCamera_Left = camera;
    }

    /// <summary>
    /// 战斗相机（右）
    /// </summary>
    [SerializeField] private CinemachineCamera BattleCamera_Right;
    public void SetBattleCameraRight(CinemachineCamera camera)
    {
        BattleCamera_Right = camera;
    }

    /// <summary>
    /// 切换当前摄像机
    /// </summary>
    public void SwitchCamera(CameraType cameraType)
    {
        Girl_Data currentCharacterData = CharacterManager.Instance.GetCurrentCharacterData;
        
        switch (cameraType)
        {
            case CameraType.DefaultCamera:
                if(CurrentCamera == DefaultCamera) return; // 如果当前已经是默认摄像机，则不切换
                CurrentCamera.Priority = 0; // 先将当前摄像机优先级降为0
                CurrentCamera = DefaultCamera;
                break;
            case CameraType.BattleCamera:
                if(CurrentCamera == BattleCamera_Left || CurrentCamera == BattleCamera_Right) return; // 如果当前已经是战斗摄像机，则不切换
                CurrentCamera.Priority = 0; // 先将当前摄像机优先级降为0
                if(EnemyInLeft())
                {
                    CurrentCamera = BattleCamera_Left;
                }
                else
                {
                    CurrentCamera = BattleCamera_Right;
                }
                break;
        }
        // 将新摄像机优先级设置为20，确保它成为当前激活的摄像机
        CurrentCamera.Priority = 20;
    }

    /// <summary>
    /// 切换到制定摄像机
    /// </summary>
    public void SwitchCamera(CinemachineCamera camera)
    {
        if (CurrentCamera == camera) return; // 如果当前已经是目标摄像机，则不切换
        CurrentCamera.Priority = 0; // 先将当前摄像机优先级降为0
        CurrentCamera = camera;
        CurrentCamera.Priority = 20; // 将新摄像机优先级设置为20，确保它成为当前激活的摄像机
    }

    /// <summary>
    /// 切换回默认摄像机
    /// </summary>
    public void SwitchToDefaultCamera()
    {
        if (CurrentCamera == DefaultCamera) return; // 如果当前已经是默认摄像机，则不切换
        CurrentCamera.Priority = 0; // 先将当前摄像机优先级降为0
        CurrentCamera = DefaultCamera;
        CurrentCamera.Priority = 20; // 将默认摄像机优先级设置为20，确保它成为当前激活的摄像机
    }



    private bool EnemyInLeft()
    {
        Vector3 CharacterPos = CharacterManager.Instance.GetCurrentCharacterData.transform.position;
        Vector3 CharacterForward = CharacterManager.Instance.GetCurrentCharacterData.transform.forward;

        Transform currentTargetEnemy = EnemyManager.Instance.currentTargetEnemy;

        if (currentTargetEnemy == null)
        {
            return true; // 如果没有目标敌人，默认使用左侧战斗摄像机
        }

        Vector3 EnemyPos = currentTargetEnemy.position;
        return Tool.IsLeft(EnemyPos, CharacterPos, CharacterForward);
    }
    
}

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
                // 计算角色和两个战斗摄像机的距离，选择较近的一个
                float distanceToLeft = Vector3.Distance(currentCharacterData.transform.position, BattleCamera_Left.transform.position);
                float distanceToRight = Vector3.Distance(currentCharacterData.transform.position, BattleCamera_Right.transform.position);
                if (distanceToLeft < distanceToRight)
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

    

    
}

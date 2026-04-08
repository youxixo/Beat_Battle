using UnityEngine;

public class PlayerReturnsPosition : MonoBehaviour
{
    [SerializeField,ChineseLabel("重生点")] private Transform returnPoint;

    private CharacterManager characterManager => CharacterManager.Instance;
    private EnemyManager enemyManager => EnemyManager.Instance;
    private CameraManager cameraManager => CameraManager.Instance;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Girl_Data playerData = characterManager.GetCurrentCharacterData;
            if(playerData != null)
            {
                var controller = playerData.GetComponent<CharacterController>();
                if(controller != null)               
                {
                    controller.enabled = false; // 禁用CharacterController以避免干扰位置重置
                }
                Vector3 newPosition = new Vector3(returnPoint.position.x, returnPoint.position.y + 5, returnPoint.position.z);
                playerData.transform.SetPositionAndRotation(newPosition, Quaternion.LookRotation(returnPoint.forward));

                controller.enabled = true; // 重新启用CharacterController
                cameraManager.SwitchToDefaultCamera();
            }
        }
        
        if(other.CompareTag("Enemy"))
        {
            int enemyID = other.gameObject.GetInstanceID();
            ZombieDate enemyData = enemyManager.GetEnemy(enemyID);
            if(enemyData != null)
            {
                enemyData.CurrentHP = 0; // 直接将敌人HP设为0，触发死亡逻辑
            }
        }
    }
}

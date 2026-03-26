using System;
using UnityEngine;

public class SelectLevelController : MonoBehaviour
{
    [SerializeField,ChineseLabel("第一关敌人")] private Transform FirstLevelEnemyPoint;
    private CharacterManager characterManager => CharacterManager.Instance;
    private EnemyManager enemyManager => EnemyManager.Instance;

    public void RetPoint(Transform point)
    {
        if(characterManager.GetCurrentCharacterData)
        {
            float y = characterManager.GetCurrentCharacterData.transform.position.y;
            characterManager.GetCurrentCharacterData.transform.position = new Vector3(point.position.x, y, point.position.z);
            characterManager.GetCurrentCharacterData.transform.rotation = Quaternion.LookRotation(point.forward);
        }
    }

    public void IsFirstLevel(bool isFirstLevel)
    {
        if(isFirstLevel)
        {
            enemyManager.currentTargetEnemy = FirstLevelEnemyPoint;
        }
        else
        {
            enemyManager.currentTargetEnemy = null;
        }
    }
}

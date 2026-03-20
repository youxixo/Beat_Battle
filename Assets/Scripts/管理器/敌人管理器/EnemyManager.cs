using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    private Dictionary<int, ZombieDate> enemyDict = new Dictionary<int, ZombieDate>();
    public int EnemyCount => enemyDict.Count;

    [SerializeField] private Transform CurrentTargetEnemy;
    /// <summary>
    /// 当前目标敌人
    /// </summary>
    public Transform currentTargetEnemy
    {
        get
        {
            if (CurrentTargetEnemy == null || !isEnemyInRange(CurrentTargetEnemy))
            {
                CurrentTargetEnemy = UpdateTargetEnemy();
            }
            return CurrentTargetEnemy;
        }
    }

    /// <summary>
    /// 注册敌人
    /// </summary>
    /// <param name="enemy"></param>
    public void RegisterEnemy(ZombieDate enemy)
    {
        if (!enemyDict.ContainsKey(enemy.gameObject.GetInstanceID()))
        {
            enemyDict.Add(enemy.gameObject.GetInstanceID(), enemy);
            Debug.Log($"注册敌人: {enemy.gameObject.name}，当前敌人数量: {enemyDict.Count}");
        }
    }

    /// <summary>
    /// 注销敌人
    /// </summary>
    /// <param name="enemy"></param>
    public void UnregisterEnemy(ZombieDate enemy)
    {
        if (enemyDict.ContainsKey(enemy.gameObject.GetInstanceID()))
        {
            enemyDict.Remove(enemy.gameObject.GetInstanceID());
        }
    }

    /// <summary>
    /// 检查当前的敌人是否可用
    /// </summary>
    private bool isEnemyInRange(Transform enemy)
    {
        if (enemy == null)
        {
            return false;
        }

        Girl_Data playerData = CharacterManager.Instance.GetCurrentCharacterData;

        float distance = Vector3.Distance(playerData.transform.position, enemy.position);
        return distance <= 18f;
    }

    /// <summary>
    /// 更新当前目标敌人
    /// </summary>
    private Transform UpdateTargetEnemy()
    {
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        if(enemyDict.Count == 0)
        {
            return null;
        }

        foreach (var enemy in enemyDict.Values)
        {
            if (enemy != null && isEnemyInRange(enemy.transform))
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }
        }

        return closestEnemy;
    }

}

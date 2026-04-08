using AYellowpaper.SerializedCollections;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField,SerializedDictionary("Enemy ID", "Enemy Data")]
    private SerializedDictionary<int, ZombieDate> enemyDict = new SerializedDictionary<int, ZombieDate>();
    public int EnemyCount => enemyDict.Count;

    [SerializeField] private Transform CurrentTargetEnemy;
    /// <summary>
    /// 当前目标敌人
    /// 如果字典中没有当前目标敌人，或者当前目标敌人不在范围内，则更新目标敌人
    /// </summary>
    public Transform currentTargetEnemy
    {
        get
        {
            if(CurrentTargetEnemy && CurrentTargetEnemy.gameObject.name == "Capsule")
            {
                return CurrentTargetEnemy;
            }
            if (CurrentTargetEnemy == null || !isEnemyInRange(CurrentTargetEnemy))
            {
                CurrentTargetEnemy = UpdateTargetEnemy();
            }
            else if(!enemyDict.ContainsKey(CurrentTargetEnemy.gameObject.GetInstanceID()))
            {
                CurrentTargetEnemy = UpdateTargetEnemy();
            }
            return CurrentTargetEnemy;
        }
        set
        {
            CurrentTargetEnemy = value;
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

    ///<summary>
    /// 清空敌人字典
    /// </summary>
    public void ClearEnemies()
    {
        enemyDict.Clear();
        CurrentTargetEnemy = null;
    }

    /// <summary>
    /// 获取特定敌人
    /// </summary>
    public ZombieDate GetEnemy(int instanceID)
    {
        if (enemyDict.TryGetValue(instanceID, out ZombieDate enemy))
        {
            return enemy;
        }
        return null;
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

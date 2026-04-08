using UnityEngine;
using UnityEngine.Events;

public class DataCollectionManager : Singleton<DataCollectionManager>
{
    [SerializeField, ChineseLabel("招式3触发次数")] private int attack3TriggerCount = 0;
    public UnityAction<int> OnAttack3TriggerCountChanged;
    /// <summary>
    /// 招式3触发次数
    /// </summary>
    public int Attack3TriggerCount
    {
        get => attack3TriggerCount;
        set
        {
            attack3TriggerCount = value;
            OnAttack3TriggerCountChanged?.Invoke(attack3TriggerCount);
        }
    }

    [SerializeField, ChineseLabel("特殊攻击触发次数")] private int specialAttackTriggerCount = 0;
    public UnityAction<int> OnSpecialAttackTriggerCountChanged;
    /// <summary>
    /// 特殊攻击触发次数
    /// </summary>
    public int SpecialAttackTriggerCount
    {
        get => specialAttackTriggerCount;
        set
        {
            specialAttackTriggerCount = value;
            OnSpecialAttackTriggerCountChanged?.Invoke(specialAttackTriggerCount);
        }
    }

    [SerializeField, ChineseLabel("眩晕触发次数")] private int dizzinessTriggerCount = 0;
    public UnityAction<int> OnDizzinessTriggerCountChanged;
    public int DizzinessTriggerCount
    {
        get => dizzinessTriggerCount;
        set
        {
            dizzinessTriggerCount = value;
            OnDizzinessTriggerCountChanged?.Invoke(dizzinessTriggerCount);
        }
    }
}

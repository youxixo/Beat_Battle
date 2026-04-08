using TMPro;
using UnityEngine;

public class DataCollectionController : MonoBehaviour
{
    [SerializeField, ChineseLabel("招式3触发次数文本")] private TextMeshProUGUI attack3TriggerCountText;
    [SerializeField, ChineseLabel("特殊攻击触发次数文本")] private TextMeshProUGUI specialAttackTriggerCountText;
    [SerializeField, ChineseLabel("眩晕触发次数文本")] private TextMeshProUGUI dizzinessTriggerCountText;

    private DataCollectionManager dataCollectionManager => DataCollectionManager.Instance;

    private void Awake()
    {
        int attack3Count = dataCollectionManager.Attack3TriggerCount;
        int specialAttackCount = dataCollectionManager.SpecialAttackTriggerCount;
        int dizzinessCount = dataCollectionManager.DizzinessTriggerCount;

        UpdateAttack3TriggerCountText(attack3Count);
        UpdateSpecialAttackTriggerCountText(specialAttackCount);
        UpdateDizzinessTriggerCountText(dizzinessCount);

        dataCollectionManager.OnAttack3TriggerCountChanged += UpdateAttack3TriggerCountText;
        dataCollectionManager.OnSpecialAttackTriggerCountChanged += UpdateSpecialAttackTriggerCountText;
        dataCollectionManager.OnDizzinessTriggerCountChanged += UpdateDizzinessTriggerCountText;
    }

    private void OnDestroy()
    {
        if(!dataCollectionManager) return;
        
        dataCollectionManager.OnAttack3TriggerCountChanged -= UpdateAttack3TriggerCountText;
        dataCollectionManager.OnSpecialAttackTriggerCountChanged -= UpdateSpecialAttackTriggerCountText;
        dataCollectionManager.OnDizzinessTriggerCountChanged -= UpdateDizzinessTriggerCountText;
    }

    /// <summary>
    /// 更新招式3触发次数文本
    /// </summary>
    public void UpdateAttack3TriggerCountText(int count)
    {
        attack3TriggerCountText.text = $"{count}";
    }

    /// <summary>
    /// 更新特殊攻击触发次数文本
    /// </summary>
    public void UpdateSpecialAttackTriggerCountText(int count)
    {
        specialAttackTriggerCountText.text = $"{count}";
    }

    /// <summary>
    /// 更新眩晕触发次数文本
    /// </summary>
    public void UpdateDizzinessTriggerCountText(int count)
    {
        dizzinessTriggerCountText.text = $"{count}";
    }
}

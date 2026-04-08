using UnityEngine;

public class SettleShowValue_LandAttack : CharacterState<LandAttackType>
{
    private Girl_Data girlData;
    public SettleShowValue_LandAttack(Girl_Data girlData): base(isGhostState: true)
    {
        this.girlData = girlData;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        
        int DodgeValue = girlData.GetDodgeValue;

        if(DodgeValue >= 2)
        {
            girlData.CurrentShowValue += 3;
        }
        else if(DodgeValue == 1)
        {
            girlData.CurrentShowValue += 2;
        }
        else
        {
            girlData.CurrentShowValue += 1;
        }
        
        girlData.SetDodgeValue(0);
        
        dataCollectionManager.Attack3TriggerCount++;
    }
}

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
        
        int jumpvalue = girlData.GetDodgeValue;

        if(jumpvalue >= 2)
        {
            girlData.CurrentShowValue += 3;
        }
        else if(jumpvalue == 1)
        {
            girlData.CurrentShowValue += 2;
        }
        else
        {
            girlData.CurrentShowValue += 1;
        }
        
        girlData.SetDodgeValue(0);
    }
}

using UnityEngine;

public class ClearDodgeValue_LandAttack : CharacterState<LandAttackType>
{
    private Girl_Data girlData;
    public ClearDodgeValue_LandAttack(Girl_Data girlData): base(isGhostState: true)
    {
        this.girlData = girlData;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        girlData.SetDodgeValue(0);
    }


}

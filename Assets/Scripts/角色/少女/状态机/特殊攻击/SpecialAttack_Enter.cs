using UnityEngine;

public class SpecialAttack_Enter : CharacterState<LandAttackType>
{
    private Girl_Data girlData;
    public SpecialAttack_Enter(Girl_Data girlData):base(isGhostState:true)
    {
        this.girlData = girlData;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        girlData.SetIsLandAttacking(true);

        // 重置地面攻击的招式类型为初始状态
        girlData.NextLandAttackType = LandAttackType.LandAttack1_Attack;

        //重置表演值
        girlData.CurrentShowValue = 0;

        // 重置节拍检测结果，避免在地面攻击状态中受到之前节拍检测结果的影响
        beatManager.CurrentBeatResult = BeatResult.none;

        dataCollectionManager.SpecialAttackTriggerCount++;
    }


}

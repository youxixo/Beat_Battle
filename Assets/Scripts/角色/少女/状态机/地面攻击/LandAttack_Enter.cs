public class LandAttack_Enter : CharacterState<LandAttackType>
{
    private Girl_Data girlData;
    public LandAttack_Enter(Girl_Data girl_Data) : base(isGhostState: true)
    {
        this.girlData = girl_Data;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        girlData.SetIsLandAttacking(true);
    }
}

public class JumpUp_Enter_Girl : CharacterState<JumpUpType>
{
    private Girl_Data girlData;
    public JumpUp_Enter_Girl(Girl_Data data):base( isGhostState:true)
    {
        girlData = data;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        int currentJumpCount = girlData.GetCurrentJumpCount;
        girlData.SetCurrentJumpCount(currentJumpCount+ 1);
    }
}

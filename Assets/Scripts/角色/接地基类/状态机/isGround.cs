public class isGround : BaseState<GroundState>
{
    private GroundCheckBaseController groundCheckController;
    public isGround(GroundCheckBaseController groundCheckController): base()
    {
        this.groundCheckController = groundCheckController;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        groundCheckController.WhenIsGround();
        groundCheckController.SetIsGround(true);
    }
}

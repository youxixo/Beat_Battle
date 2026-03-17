public class isNotGround : BaseState<GroundState>
{
    private GroundCheckBaseController groundCheckController;

    public isNotGround(GroundCheckBaseController groundCheckController): base()
    {
        this.groundCheckController = groundCheckController;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        groundCheckController.WhenIsNotGround();
        groundCheckController.SetIsGround(false);
    }
}

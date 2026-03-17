using UnityEngine;

public class WaitingInput : CharacterState<GirlStateType>
{
    public WaitingInput() : base(isGhostState: true)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }
}

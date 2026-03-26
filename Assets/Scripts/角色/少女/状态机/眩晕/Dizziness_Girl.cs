using System.Collections;
using UnityEngine;

public class Dizziness_Girl : CharacterState<GirlStateType>
{
    private Girl_Data girlData;
    private Animator animator;
    private int DizzinessHash;

    public Dizziness_Girl(Girl_Data girl_Data, Animator animator, string dizzinessHash): base()
    {
        this.girlData = girl_Data;
        this.animator = animator;
        this.DizzinessHash = Animator.StringToHash(dizzinessHash);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        girlData.CurrentHitCount = 0;

        girlData.SetAttackTapInputWindow(false);
        girlData.SetAttackHoldInputWindow(false);
        girlData.SetJumpInputWindow(false);
        girlData.SetDodgeInputWindow(false);
        girlData.SetMoveInputWindow(false);

        animator.Play(DizzinessHash);
        coroutineManager.Run("DizzinessDuration", DizzinessDuration());
    }

    public override void OnExit()
    {
        base.OnExit();

        girlData.SetAttackTapInputWindow(true);
        girlData.SetAttackHoldInputWindow(true);
        girlData.SetJumpInputWindow(true);
        girlData.SetDodgeInputWindow(true);
        girlData.SetMoveInputWindow(true);

        coroutineManager?.Stop("DizzinessDuration");
    }

    IEnumerator DizzinessDuration()
    {
        yield return new WaitForSeconds(girlData.GetDizzinessTime);
        girlData.IsDizziness = false;
    }
}

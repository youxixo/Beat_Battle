using UnityEngine;

public class InteractingReady : CharacterState<InteractingType>
{
    private Animator animator;
    private int InteractingReadyHash;

    public InteractingReady(Animator animator, string interactingReadyAnimName) : base()
    {
        this.animator = animator;
        InteractingReadyHash = Animator.StringToHash(interactingReadyAnimName);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        //角色准备交互，可以进行节拍检测
        beatManager.CurrentBeatResult = BeatResult.none; // 重置节拍结果

        beatManager.SetCharacterReadyForBeatCheck(true);

        animator.CrossFade(InteractingReadyHash, 0.2f);
    }

    public override void OnExit()
    {
        base.OnExit();
        beatManager.SetCharacterReadyForBeatCheck(false);
    }
}

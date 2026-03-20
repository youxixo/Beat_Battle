using System.Collections;
using UnityEngine;

public class InteractingStarts : CharacterState<InteractingType>
{
    private Animator animator;
    private int InteractingStartHash;

    private bool animationFinished = false;

    public InteractingStarts(Animator animator, string interactingStartAnimName) : base(needsExitTime: true,
                                                                      canExit: state => ((InteractingStarts)state).animationFinished)
    {
        this.animator = animator;
        InteractingStartHash = Animator.StringToHash(interactingStartAnimName);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        //角色正在进行交互中，不能进行节拍检测
        beatManager.CurrentBeatResult = BeatResult.none; // 重置节拍结果
        beatManager.SetCharacterReadyForBeatCheck(false);
        animationFinished = false;

        animator.Play(InteractingStartHash);
        coroutineManager.Run("InteractingStarts", InteractingStartsCoroutine());
    }

    public override void OnExit()
    {
        base.OnExit();
        coroutineManager?.Stop("InteractingStarts");
        animationFinished = false;
    }

    IEnumerator InteractingStartsCoroutine()
    {
        yield return null; // 等待一帧，确保动画状态已切换

        float animationLength = AnimatorTool.GetRealAnimationLength(animator, InteractingStartHash);
        yield return new WaitForSeconds(animationLength);
        animationFinished = true;
        fsm.StateCanExit();
    }
}

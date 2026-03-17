using System.Collections;
using UnityEngine;

public class Jump_Land_Girl : CharacterState<GirlStateType>
{
    private Animator animator;
    private int jumpLandHash;
    private bool isAnimationFinished = false;
    public Jump_Land_Girl(Animator animator, int jumpLandHash): base(needsExitTime: true, canExit: (state) => ((Jump_Land_Girl)state).isAnimationFinished)
    {
        this.animator = animator;
        this.jumpLandHash = jumpLandHash;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        isAnimationFinished = false;
        
        animator.Play(jumpLandHash);
        coroutineManager.Run("Girl_JumpLand", WaitForAnimationEnd());
    }

    public override void OnExit()
    {
        base.OnExit();

        coroutineManager?.Stop("Girl_JumpLand");
        Debug.Log("退出跳跃落地状态");
    }

    IEnumerator WaitForAnimationEnd()
    {
        yield return null;
        // 等待动画结束
        yield return new WaitForSeconds(AnimatorTool.GetAnimationLength(animator, jumpLandHash));
        Debug.Log("跳跃落地动画结束，准备退出状态");
        // 动画结束后请求退出状态
        isAnimationFinished = true;
        fsm.StateCanExit();
    }
}

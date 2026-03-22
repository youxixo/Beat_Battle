using UnityEngine;

public class BeatDetection_LandAttack : CharacterState<LandAttackType>
{
    private Animator animator;
    private Girl_Data girlData;
    private BeatCheckType beatCheckType;


    private BeatCheckType firstBeatCheckType;

    /// <summary>
    /// 双节拍检测，两个节拍出现间隔
    /// </summary>
    private float beatCheckInterval = 0.5f;

    public BeatDetection_LandAttack(
                        Animator animator, 
                        Girl_Data girlData,
                        BeatCheckType beatCheckType,
                        BeatCheckType firstBeatCheckType = BeatCheckType.JBeatCheck,
                        float beatCheckInterval = 0.5f
                        ): base()
    {
        this.animator = animator;
        this.girlData = girlData;
        this.beatCheckType = beatCheckType;
        this.firstBeatCheckType = firstBeatCheckType;
        this.beatCheckInterval = beatCheckInterval;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        animator.speed = 0f; // 暂停动画，等待节拍输入

        // 禁止输入攻击
        girlData.SetAttackTapInputWindow(false);

        beatManager?.SetCharacterReadyForBeatCheck(true); // 设置角色准备好进行节拍检测
        beatManager?.StartBeatCheck(BeatCheckType.JBeatCheck);

        if(beatCheckType == BeatCheckType.JBeatCheck)
        {
            beatManager?.StartBeatCheck(BeatCheckType.JBeatCheck);
        }
        else if(beatCheckType == BeatCheckType.KBeatCheck)
        {
            beatManager?.StartBeatCheck(BeatCheckType.KBeatCheck);
        }
        else if(beatCheckType == BeatCheckType.BothCheck)
        {
            beatManager?.StartBothBeatCheck(firstBeatCheckType, beatCheckInterval);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        //恢复动画播放速度
        animator.speed = 1f;

        beatManager?.SetCharacterReadyForBeatCheck(false); // 退出状态时重置角色节拍检测准备状态

        girlData?.SetAttackTapInputWindow(true); // 恢复攻击输入窗口

        // 宣布节拍检测结束，触发相关事件
        beatManager?.StopBeatCheckAction?.Invoke();
    }
}

using UnityEngine;

public class BeatDetection_LandAttack : CharacterState<LandAttackType>
{
    private GirlController characterController;
    private Animator animator;
    private Girl_Data girlData;
    private BeatCheckType beatCheckType;
    private AudioClip goodBeatAudioClip;
    private AudioClip secondBeatGoodAudioClip;

    private BeatCheckType firstBeatCheckType;

    /// <summary>
    /// 双节拍检测，两个节拍出现间隔
    /// </summary>
    private float beatCheckInterval = 0.5f;

    public BeatDetection_LandAttack(
                        GirlController characterController,
                        Animator animator, 
                        Girl_Data girlData,
                        BeatCheckType beatCheckType,
                        AudioClip goodBeatAudioClip,
                        BeatCheckType firstBeatCheckType = BeatCheckType.JBeatCheck,
                        float beatCheckInterval = 0.5f,
                        AudioClip secondBeatGoodAudioClip = null
                        ): base()
    {
        this.characterController = characterController;
        this.animator = animator;
        this.girlData = girlData;
        this.beatCheckType = beatCheckType;
        this.goodBeatAudioClip = goodBeatAudioClip;
        this.firstBeatCheckType = firstBeatCheckType;
        this.beatCheckInterval = beatCheckInterval;
        this.secondBeatGoodAudioClip = secondBeatGoodAudioClip;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        animator.speed = 0f; // 暂停动画，等待节拍输入

        // 禁止输入攻击
        girlData.SetAttackTapInputWindow(false);
        girlData.SetDodgeInputWindow(false);
        girlData.SetIsLandAttacking(true);

        beatManager?.SetCharacterReadyForBeatCheck(true); // 设置角色准备好进行节拍检测
        beatManager?.StartBeatCheck(BeatCheckType.JBeatCheck);

        if(beatCheckType == BeatCheckType.JBeatCheck)
        {
            beatManager.JBeatCheckResultAction += PlayGoodBeat;
            beatManager?.StartBeatCheck(BeatCheckType.JBeatCheck);
        }
        else if(beatCheckType == BeatCheckType.KBeatCheck)
        {
            beatManager.KBeatCheckResultAction += PlayGoodBeat;
            beatManager?.StartBeatCheck(BeatCheckType.KBeatCheck);
        }
        else if(beatCheckType == BeatCheckType.BothCheck)
        {
            switch (firstBeatCheckType)
            {
                case BeatCheckType.JBeatCheck:
                    beatManager.JBeatCheckResultAction += PlayGoodBeat;
                    beatManager.KBeatCheckResultAction += PlaySecondGoodBeat;
                    break;
                case BeatCheckType.KBeatCheck:
                    beatManager.KBeatCheckResultAction += PlayGoodBeat;
                    beatManager.JBeatCheckResultAction += PlaySecondGoodBeat;
                    break;
            }
            beatManager?.StartBothBeatCheck(firstBeatCheckType, beatCheckInterval);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        //恢复动画播放速度
        animator.speed = 1f;

        beatManager?.SetCharacterReadyForBeatCheck(false); // 退出状态时重置角色节拍检测准备状态
        beatManager?.StopBeatCheckAction?.Invoke();

        if(beatManager)
        {
            beatManager.JBeatCheckResultAction -= PlayGoodBeat;
            beatManager.KBeatCheckResultAction -= PlayGoodBeat;
            beatManager.JBeatCheckResultAction -= PlaySecondGoodBeat;
            beatManager.KBeatCheckResultAction -= PlaySecondGoodBeat;
        }

        girlData?.SetAttackTapInputWindow(true); // 恢复攻击输入窗口
        girlData?.SetDodgeInputWindow(true); // 恢复闪避输入窗口
        girlData?.SetIsLandAttacking(false); // 重置地面攻击状态
        // 宣布节拍检测结束，触发相关事件
    }

    private void PlayGoodBeat(BeatResult beatResult)
    {
        if(beatResult == BeatResult.Good)
        {
            characterController.PlayAttackAudio(goodBeatAudioClip);
        }
    }

    private void PlaySecondGoodBeat(BeatResult beatResult)
    {
        if(beatResult == BeatResult.Good)
        {
            characterController.PlayAttackAudio(secondBeatGoodAudioClip);
        }
    }
}

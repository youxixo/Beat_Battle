using UnityEngine;
using UnityEngine.UI;

public class BGMBeatController : MonoBehaviour
{
    [SerializeField, ChineseLabel("音频源")] private AudioSource audioSource;
    [SerializeField, ChineseLabel("节拍时间线")] private BeatTimelineSO beatTimeline;

    [SerializeField, ChineseLabel("节拍图片")] private Image BeatImage;
    [SerializeField, ChineseLabel("节拍颜色")] private Color WhenBeatColor = Color.red;
    [SerializeField, ChineseLabel("普通颜色")] private Color WhenNotBeatColor = Color.black;

    [Header("时间控制")]
    [SerializeField, ChineseLabel("节拍持续时间")] private float BeatDuration = 0.1f;
    [SerializeField, ChineseLabel("非节拍冷却时间")] private float NotBeatDuration = 0.2f;

    private int nextBeatIndex = 0;
    private float lastTime = 0f;

    private DownTimer stateTimer;
    private bool isInBeatState = false;

    private MultiTimerManager timerManager => MultiTimerManager.Instance;
    private BeatManager beatManager => BeatManager.Instance;

    void OnEnable()
    {
        stateTimer = timerManager.Create_DownTimer("BGMStateTimer_" + gameObject.GetInstanceID(), 0f);
    }

    void Update()
    {
        if (audioSource == null || beatTimeline == null || beatTimeline.beatTimes.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name}：音频或节拍数据未设置");
            return;
        }

        float currentTime = audioSource.time;


        if (currentTime < lastTime)
        {
            nextBeatIndex = 0;
        }

        lastTime = currentTime;

        UpdateState();

   
        while (nextBeatIndex < beatTimeline.beatTimes.Count &&
               currentTime >= beatTimeline.beatTimes[nextBeatIndex])
        {
            TryTriggerBeat();
            nextBeatIndex++;
        }
    }

    /// <summary>
    /// 状态机更新
    /// </summary>
    void UpdateState()
    {
        if (!stateTimer.IsComplete()) return;

        if (isInBeatState)
        {
            // 红 → 黑
            isInBeatState = false;

            if (BeatImage != null)
            {
                beatManager.IsBGMInBeat = false;
                BeatImage.color = WhenNotBeatColor;
            }
                

            stateTimer.ResetTimer(NotBeatDuration);
            stateTimer.StartTimer();
        }
    }

    /// <summary>
    /// 尝试触发节拍
    /// </summary>
    void TryTriggerBeat()
    {
        // ❗只有在“空闲状态”才能触发
        if (!stateTimer.IsComplete() || isInBeatState)
            return;

        // 进入节拍状态
        isInBeatState = true;

        if (BeatImage != null)
        {
            beatManager.IsBGMInBeat = true;
            BeatImage.color = WhenBeatColor;
        }
            

        stateTimer.ResetTimer(BeatDuration);
        stateTimer.StartTimer();
    }

    void OnDisable()
    {
        if (timerManager != null)
        {
            timerManager.Delete_DownTimer("BGMStateTimer_" + gameObject.GetInstanceID());
        }
    }
}
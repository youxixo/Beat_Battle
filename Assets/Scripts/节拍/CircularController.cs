using System.Collections;
using UnityEngine;


public class CircularController : MonoBehaviour
{
    [SerializeField] private BeatCheckType BeatType;

    [SerializeField] private RectTransform OutCircle;

    [SerializeField] protected RectTransform InCircle;
    
    [Header("节拍设置")]
    [Header("节拍速度")]
    [SerializeField]private float beatSpeed;

    [Header("外圆和内圆大小差距范围（单位：像素）")]
    [SerializeField] private float beatRange;

    [Header("InCircle最终大小（单位：像素）")]
    [SerializeField] private float InCircleFinalSize;

    [Header("显示出来后，几秒后才正式开始检测节拍，单位：秒")]
    [SerializeField] private float beatCheckDelayTime = 0.5f;

    private Vector3 inCircleSize;
    private bool isBeatActive;

    private InputManager inputManager => InputManager.Instance;
    private BeatManager beatManager => BeatManager.Instance;
    private CoroutineManager coroutineManager => CoroutineManager.Instance;

    private void Awake()
    {
        InCircle.localScale = Vector3.zero;
        inCircleSize = new Vector3(InCircleFinalSize, InCircleFinalSize, InCircleFinalSize);
    }

    void OnEnable()
    {

        coroutineManager.Run("BeatCheckDelay" + gameObject.GetInstanceID(), BeatCheckDelayCoroutine());
    }

    private void Update()
    {
        if (!isBeatActive)
        {
            return;
        }

        InCircle.localScale = InCircle.localScale + Vector3.one * beatSpeed * Time.deltaTime;

        if (InCircle.localScale.x >= inCircleSize.x)
        {
            isBeatActive = false;
            switch (BeatType)
            {
                case BeatCheckType.JBeatCheck:
                    beatManager.JBeatCheckResult = BeatResult.Miss;
                    break;
                case BeatCheckType.KBeatCheck:
                    beatManager.KBeatCheckResult = BeatResult.Miss;
                    break;
            }
            gameObject.SetActive(false);
        }
    }

    

    void OnDisable()
    {
        if (inputManager != null)
        {
            inputManager.AttackTapEvent -= CheckBeat;
            inputManager.DodgeEvent -= CheckBeat;
        }

        if(coroutineManager)
        {
            coroutineManager.Stop("BeatCheckDelay" + gameObject.GetInstanceID());
        }
        isBeatActive = false;

        InCircle.localScale = Vector3.zero;
    }

    IEnumerator BeatCheckDelayCoroutine()
    {
        yield return new WaitForSeconds(beatCheckDelayTime);
        switch (BeatType)
        {
            case BeatCheckType.JBeatCheck:
                inputManager.AttackTapEvent += CheckBeat;
                break;
            case BeatCheckType.KBeatCheck:
                inputManager.DodgeEvent += CheckBeat;
                break;
        }
        isBeatActive = true;
    }

    private void CheckBeat()
    {
        isBeatActive = false;
        Vector3 inCircleSize = InCircle.localScale;
        Vector3 outCircleSize = OutCircle.localScale;

        if (Mathf.Abs(inCircleSize.x - outCircleSize.x) <= beatRange)
        {
            // 在这里执行击打成功的逻辑
            switch (BeatType)
            {
                case BeatCheckType.JBeatCheck:
                    beatManager.JBeatCheckResult = BeatResult.Good;
                    break;
                case BeatCheckType.KBeatCheck:
                    beatManager.KBeatCheckResult = BeatResult.Good;
                    break;
            }
        }
        else
        {
            switch (BeatType)
            {
                case BeatCheckType.JBeatCheck:
                    beatManager.JBeatCheckResult = BeatResult.Miss;
                    break;
                case BeatCheckType.KBeatCheck:
                    beatManager.KBeatCheckResult = BeatResult.Miss;
                    break;
            }
        }

        gameObject.SetActive(false);
    }
}

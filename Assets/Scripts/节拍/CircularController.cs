using UnityEngine;

public class CircularController : MonoBehaviour
{
    [SerializeField] private RectTransform OutCircle;

    [SerializeField] protected RectTransform InCircle;
    
    [Header("节拍设置")]
    [Header("节拍速度")]
    [SerializeField]private float beatSpeed;

    [Header("外圆和内圆大小差距范围（单位：像素）")]
    [SerializeField] private float beatRange;

    [Header("InCircle最终大小（单位：像素）")]
    [SerializeField] private float InCircleFinalSize;

    private Vector3 inCircleSize;
    private bool isBeatActive;

    private InputManager inputManager => InputManager.Instance;
    private BeatManager beatManager => BeatManager.Instance;

    private void Awake()
    {
        InCircle.localScale = Vector3.zero;
        inCircleSize = new Vector3(InCircleFinalSize, InCircleFinalSize, InCircleFinalSize);
    }

    void OnEnable()
    {
        inputManager.AttackTapEvent += CheckBeat;
        isBeatActive = true;
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
            beatManager.CurrentBeatResult = BeatResult.Miss;
            gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        if (inputManager != null)
        {
            inputManager.AttackTapEvent -= CheckBeat;
        }

        InCircle.localScale = Vector3.zero;
    }

    private void CheckBeat()
    {
        isBeatActive = false;
        Vector3 inCircleSize = InCircle.localScale;
        Vector3 outCircleSize = OutCircle.localScale;

        if (Mathf.Abs(inCircleSize.x - outCircleSize.x) <= beatRange)
        {
            // 在这里执行击打成功的逻辑
            beatManager.CurrentBeatResult = BeatResult.Good;
        }
        else
        {
            // 在这里执行击打失败的逻辑
            beatManager.CurrentBeatResult = BeatResult.Miss;
        }

        gameObject.SetActive(false);
    }
}

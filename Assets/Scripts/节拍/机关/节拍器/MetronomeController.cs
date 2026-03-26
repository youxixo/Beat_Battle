using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class MetronomeController : MonoBehaviour
{
    [SerializeField] private GameObject BeatNotFixedEffect;
    [SerializeField] private Transform CharacterStandTransform;
    [SerializeField] private CinemachineCamera MetronomeCamera;
    [SerializeField,ChineseLabel("节拍点")] private MetronomePoint[] Points;
    
    [SerializeField] private UnityEvent WhenPlayClose;
    [SerializeField] private UnityEvent WhenPlayLeaved;
    [SerializeField] private UnityEvent WhenPlayInteracted;

    [SerializeField] private bool TestMode = false;

    private int currentBeatPointIndex = 0;
    private bool IsInteracting = false;
    private AudioSource[] audioSources;

    private InputManager inputManager => InputManager.Instance;
    private CameraManager cameraManager => CameraManager.Instance;
    private CharacterManager characterManager => CharacterManager.Instance;
    private BeatManager beatManager => BeatManager.Instance;
    private CoroutineManager coroutineManager => CoroutineManager.Instance;

    private void Awake()
    {
        audioSources = GetComponents<AudioSource>();
        if (audioSources.Length == 0)
        {
            Debug.LogError("没有找到AudioSource组件！");
            return;
        }
        BeatNotFixedEffect?.SetActive(false);
    }

    #if UNITY_EDITOR
    void Update()
    {
        if(!TestMode) return;
        Girl_Data currentCharacterData = characterManager.GetCurrentCharacterData;

        if(currentCharacterData)
        {
            currentCharacterData.transform.position = new Vector3(CharacterStandTransform.position.x, currentCharacterData.transform.position.y, CharacterStandTransform.position.z);
        }
    }
    #endif

    void OnDisable()
    {
        if(inputManager)
        {
            inputManager.InteractEvent -= StartMetronomeInteraction;
        }
        if(beatManager)
        {
            beatManager.BeatCheckResultAction -= HitChecker;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inputManager.InteractEvent -= StartMetronomeInteraction;
            inputManager.InteractEvent += StartMetronomeInteraction;
            beatManager.BeatCheckResultAction += HitChecker;
            WhenPlayClose?.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsInteracting) return;
        if (other.CompareTag("Player"))
        {
            inputManager.InteractEvent -= StartMetronomeInteraction;
            beatManager.BeatCheckResultAction -= HitChecker;

            IsInteracting = false;
            WhenPlayLeaved?.Invoke();
        }
    }

    /// <summary>
    /// 玩家开始与节拍器交互
    /// </summary>
    public void StartMetronomeInteraction()
    {
        if (currentBeatPointIndex >= Points.Length) return;

        Girl_Data currentCharacterData = characterManager.GetCurrentCharacterData;

        // 将角色状态设置为正在交互，以防止误触发离开事件
        currentCharacterData.SetIsInteracting(true);

        IsInteracting = true;
        cameraManager.SwitchCamera(MetronomeCamera);

        // 将玩家传送到节拍器前
        Vector3 OldCharacterPosition = currentCharacterData.transform.position;
        Vector3 NewCharacterPosition = new(CharacterStandTransform.position.x, OldCharacterPosition.y, CharacterStandTransform.position.z);
        currentCharacterData.transform.SetPositionAndRotation(NewCharacterPosition, Quaternion.LookRotation(CharacterStandTransform.forward));

        // 处理UI显示
        WhenPlayInteracted?.Invoke();

        if(currentCharacterData.IsInteracting)
        {
            coroutineManager.Run("Metronome" + gameObject.GetInstanceID(), ResetBeatCheckCoroutine());
        }
    }

     /// <summary>
    /// 玩家击打节拍器的结果处理
    /// </summary>
    private void HitChecker(BeatResult beatResult)
    {
        beatManager.JBeatCheckResultAction -= Play_First_AudioClip;
        beatManager.KBeatCheckResultAction -= Play_First_AudioClip;
        beatManager.JBeatCheckResultAction -= Play_Second_AudioClip;
        beatManager.KBeatCheckResultAction -= Play_Second_AudioClip;

        MetronomePoint currentPoint = Points[currentBeatPointIndex];
        PointBeatController currentBeatPoint = currentPoint.PointBeatController;
        if(currentBeatPoint.GetBeatPointState == BeatPointType.Bad)
        {
            WhenPlayerHitBadBeat();
            return;
        }

        if (beatResult == BeatResult.Good)
        {
            currentBeatPointIndex++;
            currentBeatPoint.HitPoint();

            if(currentBeatPointIndex >= Points.Length)
            {
                FinishInteraction();
                WhenPlayerFinishBeat();
            }
            else
            {
                // 继续击打下一个节拍点，重置节拍检测
                beatManager.SetCharacterReadyForBeatCheck(false);

                if(TestMode)
                {
                    return;
                }
                coroutineManager.Run("Metronome" + gameObject.GetInstanceID(), ResetBeatCheckCoroutine());
            }
        }
        else
        {
            ResetBeatCheck();
            FinishInteraction();
        }

    }

    /// <summary>
    /// 如果玩家打到坏节拍, 退出交互，重置节拍，打开UI提示
    /// </summary>
    private void WhenPlayerHitBadBeat()
    {
        FinishInteraction();
        ResetBeatCheck();
        coroutineManager.Run("MetronomeUI" + gameObject.GetInstanceID(), OpenBeatNotFixedEffect());
    }

    /// <summary>
    /// 重置节拍检测
    /// </summary>
    private void ResetBeatCheck()
    {
        currentBeatPointIndex = 0;
        foreach (var beatPoint in Points)
        {
            if(beatPoint.PointBeatController.GetBeatPointState == BeatPointType.Hited)
            {
                beatPoint.PointBeatController.ChangeState(BeatPointType.Good);
            }
        }
        IsInteracting = false;
    }

    /// <summary>
    /// 结束交互
    /// </summary>
    private void FinishInteraction()
    {
        Girl_Data currentCharacterData = characterManager.GetCurrentCharacterData;

        cameraManager.SwitchToDefaultCamera();
        currentCharacterData.SetIsInteracting(false);


        IsInteracting = false;
        inputManager.InteractEvent -= StartMetronomeInteraction;
        beatManager.BeatCheckResultAction -= HitChecker;
        WhenPlayLeaved?.Invoke();
    }

    /// <summary>
    /// 完成节拍的效果
    /// </summary>
    private void WhenPlayerFinishBeat()
    {
        gameObject.SetActive(false);
    }

    IEnumerator ResetBeatCheckCoroutine()
    {
        BeatCheckType currentBeatCheckType = Points[currentBeatPointIndex].beatType.BeatCheckType;
        switch(currentBeatCheckType)
        {
            case BeatCheckType.JBeatCheck:
                beatManager.JBeatCheckResultAction += Play_First_AudioClip;
                break;
            case BeatCheckType.KBeatCheck:
                beatManager.KBeatCheckResultAction += Play_First_AudioClip;
                break;
            case BeatCheckType.BothCheck:
                switch(Points[currentBeatPointIndex].beatType.FirstBeatCheckType)
                {
                    case BeatCheckType.JBeatCheck:
                        beatManager.JBeatCheckResultAction += Play_First_AudioClip;
                        beatManager.KBeatCheckResultAction += Play_Second_AudioClip;
                        break;
                    case BeatCheckType.KBeatCheck:
                        beatManager.KBeatCheckResultAction += Play_First_AudioClip;
                        beatManager.JBeatCheckResultAction += Play_Second_AudioClip;
                        break;
                }
                break;
        }

        yield return new WaitUntil(() => beatManager.CharacterReadyForBeatCheck);

        if(currentBeatCheckType == BeatCheckType.BothCheck)
        {
            beatManager.StartBothBeatCheck(Points[currentBeatPointIndex].beatType.FirstBeatCheckType, Points[currentBeatPointIndex].beatType.IntervalBetweenTwoBeats);
        }
        else
        {
            beatManager.StartBeatCheck(currentBeatCheckType);
        }
    }

    IEnumerator OpenBeatNotFixedEffect()
    {
        BeatNotFixedEffect.SetActive(true);
        yield return new WaitForSeconds(3f);
        BeatNotFixedEffect.SetActive(false);
    }

    private int currentBeatIndex = 0;
    ///<summary>
    /// 播放第一个音效
    /// </summary>
    public void Play_First_AudioClip(BeatResult beatResult)
    {
        if(beatResult == BeatResult.Good)
        {
            if (audioSources.Length > 0 && currentBeatPointIndex < Points.Length)
            {
                audioSources[currentBeatIndex].Stop();
                audioSources[currentBeatIndex].clip = Points[currentBeatPointIndex].beatType.BeatAudioClip;
                audioSources[currentBeatIndex].Play();
                currentBeatIndex = (currentBeatIndex + 1) % audioSources.Length;
            }
        }
    }

    /// <summary>
    /// 播放第二个音效
    /// </summary>
    public void Play_Second_AudioClip(BeatResult beatResult)
    {
        if(beatResult == BeatResult.Good)
        {
            if (audioSources.Length > 0 && currentBeatPointIndex < Points.Length)
            {
                audioSources[currentBeatIndex].Stop();
                audioSources[currentBeatIndex].clip = Points[currentBeatPointIndex].beatType.SecondBeatAudioClip;
                audioSources[currentBeatIndex].Play();
                currentBeatIndex = (currentBeatIndex + 1) % audioSources.Length;
            }
        }
    }
}

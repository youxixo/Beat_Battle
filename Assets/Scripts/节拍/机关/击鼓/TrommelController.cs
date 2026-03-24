using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class TrommelController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera TrommelCamera;
    [SerializeField] private Transform CharacterStandTransform;
    [SerializeField] private TextMeshProUGUI NumberText;
    [SerializeField] private BeatType[] BeatTypes;
    [SerializeField] private int CurrentHits = 0;
    [SerializeField] private GameObject Door;
    [SerializeField]private UnityEvent WhenPlayerCloseToTrommel;
    [SerializeField]private UnityEvent WhenPlayerStartHitTrommel;
    [SerializeField]private UnityEvent WhenPlayerLeaveTrommel;

    private AudioSource[] audioSources;
    private bool IsInteracting = false;
    private InputManager inputManager => InputManager.Instance;
    private CameraManager cameraManager => CameraManager.Instance;
    private CharacterManager characterManager => CharacterManager.Instance;
    private BeatManager beatManager => BeatManager.Instance;
    private CoroutineManager coroutineManager => CoroutineManager.Instance;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        audioSources = GetComponents<AudioSource>();
        if (audioSources.Length == 0)
        {
            Debug.LogError("没有找到AudioSource组件！");
            return;
        }
        NumberText.text = $"{CurrentHits}/{BeatTypes.Length}";
    }

    void OnTriggerEnter(Collider other)
    {
        // 如果已经完成击鼓，直接返回，避免误触进入事件
        if(CurrentHits >= BeatTypes.Length) return;

        if (other.CompareTag("Player"))
        {
            inputManager.InteractEvent -= StartHitTrommel;
            inputManager.InteractEvent += StartHitTrommel;
            beatManager.BeatCheckResultAction += HitChecker;
            WhenPlayerCloseToTrommel?.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 如果正在交互中，直接返回，避免误触退出事件
        if(IsInteracting) return;
        if (other.CompareTag("Player"))
        {
            inputManager.InteractEvent -= StartHitTrommel;
            beatManager.BeatCheckResultAction -= HitChecker;
            WhenPlayerLeaveTrommel?.Invoke();
        }
    }

    private void StartHitTrommel()
    {
        Girl_Data currentCharacterData = characterManager.GetCurrentCharacterData;
        if (TrommelCamera == null)
        {
            Debug.LogError("击鼓相机没设置！");
            return;
        }

        currentCharacterData.SetIsInteracting(true);


        IsInteracting = true;
        cameraManager.SwitchCamera(TrommelCamera);
        Vector3 OldCharacterPosition = currentCharacterData.transform.position;
        Vector3 NewCharacterPosition = new(CharacterStandTransform.position.x, OldCharacterPosition.y, CharacterStandTransform.position.z);
        currentCharacterData.transform.SetPositionAndRotation(NewCharacterPosition, Quaternion.LookRotation(CharacterStandTransform.forward));
        
        WhenPlayerStartHitTrommel?.Invoke();

        // 开启节拍检测
        if(currentCharacterData.IsInteracting)
        {
            coroutineManager.Run("ResetBeatCheck" + gameObject.GetInstanceID(), ResetBeatCheckCoroutine());
        }
    }

    private void HitChecker(BeatResult beatResult)
    {
        // 取消之前注册的节拍检测结果事件，避免重复触发
        beatManager.JBeatCheckResultAction -= Play_First_AudioClip;
        beatManager.KBeatCheckResultAction -= Play_First_AudioClip;
        beatManager.JBeatCheckResultAction -= Play_Second_AudioClip;
        beatManager.KBeatCheckResultAction -= Play_Second_AudioClip;

        if (beatResult == BeatResult.Good)
        {
            CurrentHits++;
            NumberText.text = $"{CurrentHits}/{BeatTypes.Length}";
        }

        if(CurrentHits >= BeatTypes.Length)
        {
            // 胜利
            Finish();
        }
        else
        {
            beatManager.SetCharacterReadyForBeatCheck(false);
            // 继续击鼓，重置节拍检测
            coroutineManager.Run("ResetBeatCheck" + gameObject.GetInstanceID(), ResetBeatCheckCoroutine());
        }
    }

    private void Finish()
    {
        Girl_Data currentCharacterData = characterManager.GetCurrentCharacterData;

        cameraManager.SwitchToDefaultCamera();
        currentCharacterData.SetIsInteracting(false);


        IsInteracting = false;
        inputManager.InteractEvent -= StartHitTrommel;
        beatManager.BeatCheckResultAction -= HitChecker;
        WhenPlayerLeaveTrommel?.Invoke();
        Door.SetActive(false);
    }

    IEnumerator ResetBeatCheckCoroutine()
    {
        switch(BeatTypes[CurrentHits].BeatCheckType)
        {
            case BeatCheckType.JBeatCheck:
                beatManager.JBeatCheckResultAction += Play_First_AudioClip;
                break;
            case BeatCheckType.KBeatCheck:
                beatManager.KBeatCheckResultAction += Play_First_AudioClip;
                break;
            case BeatCheckType.BothCheck:
                switch (BeatTypes[CurrentHits].FirstBeatCheckType)                
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
        if(BeatTypes[CurrentHits].BeatCheckType == BeatCheckType.BothCheck)
        {
            beatManager.StartBothBeatCheck(BeatTypes[CurrentHits].FirstBeatCheckType, BeatTypes[CurrentHits].IntervalBetweenTwoBeats);
        }
        else
        {
            beatManager.StartBeatCheck(BeatTypes[CurrentHits].BeatCheckType);
        }
    }

    private int currentBeatIndex = 0;
    ///<summary>
    /// 播放第一个音效
    /// </summary>
    public void Play_First_AudioClip(BeatResult beatResult)
    {
        if(beatResult == BeatResult.Good)
        {
            if (audioSources.Length > 0)
            {
                audioSources[currentBeatIndex].Stop();
                audioSources[currentBeatIndex].clip = BeatTypes[CurrentHits].BeatAudioClip;
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
            if (audioSources.Length > 0)
            {
                audioSources[currentBeatIndex].Stop();
                audioSources[currentBeatIndex].clip = BeatTypes[CurrentHits].SecondBeatAudioClip;
                audioSources[currentBeatIndex].Play();
                currentBeatIndex = (currentBeatIndex + 1) % audioSources.Length;
            }
        }
    }
}

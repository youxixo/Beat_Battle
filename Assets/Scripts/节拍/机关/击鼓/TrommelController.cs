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
    [SerializeField] private int NumberOfHitsToWin = 5;
    [SerializeField] private int CurrentHits = 0;
    [SerializeField] private GameObject Door;
    [SerializeField]private UnityEvent WhenPlayerCloseToTrommel;
    [SerializeField]private UnityEvent WhenPlayerStartHitTrommel;
    [SerializeField]private UnityEvent WhenPlayerLeaveTrommel;

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
        NumberText.text = $"{CurrentHits}/{NumberOfHitsToWin}";
    }

    void OnTriggerEnter(Collider other)
    {
        // 如果已经完成击鼓，直接返回，避免误触进入事件
        if(CurrentHits >= NumberOfHitsToWin) return;

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
        if (beatResult == BeatResult.Good)
        {
            CurrentHits++;
            NumberText.text = $"{CurrentHits}/{NumberOfHitsToWin}";
        }

        if(CurrentHits >= NumberOfHitsToWin)
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
        yield return new WaitUntil(() => beatManager.CharacterReadyForBeatCheck);
        beatManager.StartBeatCheckAction?.Invoke();
    }
}

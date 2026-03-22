using UnityEngine;
using UnityEngine.Events;

public class FixPointController : MonoBehaviour
{
    [SerializeField] private bool OnTheWall = true;
    [SerializeField] private PointBeatController BeatPointController;

    [SerializeField] private UnityEvent WhenPlayerClose;

    [SerializeField] private UnityEvent WhenPlayerInteract;

    [SerializeField] private UnityEvent WhenPlayerLeave;

    private InputManager inputManager => InputManager.Instance;

    private void OnTriggerEnter(Collider other)
    {
        // 如果已经修复，直接返回，避免误触进入事件
        if (OnTheWall) return;

        if (other.CompareTag("Player"))
        {
            inputManager.InteractEvent -= StartFix;
            inputManager.InteractEvent += StartFix;
            WhenPlayerClose?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 如果在墙上，直接返回，避免误触退出事件
        if (OnTheWall) return;

        if (other.CompareTag("Player"))
        {
            inputManager.InteractEvent -= StartFix;
            WhenPlayerLeave?.Invoke();
        }
    }

    private void StartFix()
    {
        BeatPointController.FixBadPoint();
        OnTheWall = true;
        WhenPlayerInteract?.Invoke();
    }

    public void SetPointOnTheWall(bool value)
    {
        OnTheWall = value;
    }
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (BeatPointController == null)
        {
            BeatPointController = GetComponentInParent<PointBeatController>();
        }
    }
    #endif
}

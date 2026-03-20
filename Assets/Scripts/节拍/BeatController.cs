using UnityEngine;

public class BeatController : MonoBehaviour
{
    [SerializeField] private GameObject Beat;

    private BeatManager beatManager => BeatManager.Instance;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        Beat.SetActive(false);
    }

    void OnEnable()
    {
        beatManager.StartBeatCheckAction += OpenBeatCheck;
        beatManager.StopBeatCheckAction += CloseBeatCheck;
    }

    void OnDisable()
    {
        if (beatManager != null)
        {
            beatManager.StartBeatCheckAction -= OpenBeatCheck;
            beatManager.StopBeatCheckAction -= CloseBeatCheck;
        }
    }

    private void OnDestroy()
    {
        if (beatManager != null)
        {
            beatManager.StartBeatCheckAction -= OpenBeatCheck;
        }
    }

    private void OpenBeatCheck()
    {
        Beat.SetActive(true);
    }

    private void CloseBeatCheck()
    {
        Beat.SetActive(false);
    }

}

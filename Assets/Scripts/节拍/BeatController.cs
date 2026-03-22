using System.Collections;
using UnityEngine;

public class BeatController : MonoBehaviour
{
    [SerializeField] private GameObject JBeatCheck;
    [SerializeField] private GameObject KBeatCheck;

    private BeatManager beatManager => BeatManager.Instance;
    private CoroutineManager coroutineManager => CoroutineManager.Instance;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        JBeatCheck?.SetActive(false);
        KBeatCheck?.SetActive(false);
    }

    void OnEnable()
    {
        beatManager.JBeatStartCheckAction += OpenJBeatCheck;
        beatManager.KBeatStartCheckAction += OpenKBeatCheck;
        beatManager.BothBeatStartCheckAction += OpenBothBeatCheck;

        beatManager.StopBeatCheckAction += CloseBeatCheck;
    }

    void OnDisable()
    {
        if (beatManager != null)
        {
            beatManager.JBeatStartCheckAction -= OpenJBeatCheck;
            beatManager.KBeatStartCheckAction -= OpenKBeatCheck;
            beatManager.BothBeatStartCheckAction -= OpenBothBeatCheck;
            
            beatManager.StopBeatCheckAction -= CloseBeatCheck;
        }

        if(coroutineManager)
        {
            coroutineManager.Stop("WaitAndOpenKBeatCheck" + gameObject.GetInstanceID());
            coroutineManager.Stop("WaitAndOpenJBeatCheck" + gameObject.GetInstanceID());
        }
    }

    private void OnDestroy()
    {
        if (beatManager != null)
        {
            beatManager.JBeatStartCheckAction -= OpenJBeatCheck;
            beatManager.KBeatStartCheckAction -= OpenKBeatCheck;
            beatManager.BothBeatStartCheckAction -= OpenBothBeatCheck;

            beatManager.StopBeatCheckAction -= CloseBeatCheck;
        }

        if(coroutineManager)
        {
            coroutineManager.Stop("WaitAndOpenKBeatCheck" + gameObject.GetInstanceID());
            coroutineManager.Stop("WaitAndOpenJBeatCheck" + gameObject.GetInstanceID());
        }
    }

    private void OpenJBeatCheck()
    {
        JBeatCheck?.SetActive(true);
    }

    private void OpenKBeatCheck()
    {
        KBeatCheck.SetActive(true);
    }

    private void OpenBothBeatCheck(BeatCheckType FirstCheckType,float interval)
    {
        if(FirstCheckType == BeatCheckType.JBeatCheck)
        {
            OpenJBeatCheck();
            coroutineManager.Run("WaitAndOpenKBeatCheck" + gameObject.GetInstanceID(), WaitAndOpenKBeatCheck(interval));
        }
        else if(FirstCheckType == BeatCheckType.KBeatCheck)
        {
            OpenKBeatCheck();
            coroutineManager.Run("WaitAndOpenJBeatCheck" + gameObject.GetInstanceID(), WaitAndOpenJBeatCheck(interval));
        }
    }
    
    private void CloseBeatCheck()
    {
        JBeatCheck?.SetActive(false);
        KBeatCheck?.SetActive(false);
    }

    private IEnumerator WaitAndOpenKBeatCheck(float interval)
    {
        yield return new WaitForSeconds(interval);
        OpenKBeatCheck();
    }

    private IEnumerator WaitAndOpenJBeatCheck(float interval)
    {
        yield return new WaitForSeconds(interval);
        OpenJBeatCheck();
    }

}

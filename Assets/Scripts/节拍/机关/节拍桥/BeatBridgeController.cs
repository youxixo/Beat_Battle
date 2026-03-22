using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BeatBridgeController : MonoBehaviour
{
    [SerializeField] private AudioSource BeatPlayAudioSource;

    /// <summary>
    /// 前奏音效列表
    /// </summary>
    [SerializeField] private AudioClip[] PreludeClips;

    /// <summary>
    /// 前奏间隔
    /// </summary>
    [SerializeField] private float PreludeInterval;

    /// <summary>
    /// 最终节拍音效
    /// </summary>
    [SerializeField] private AudioClip FinalBeatClip;

    /// <summary>
    /// 播放最终节拍音效的延迟时间
    /// </summary>
    [SerializeField] private float FinalBeatDelay;

    /// <summary>
    /// 桥梁对象，节拍结束后隐藏
    /// </summary>
    [SerializeField] private GameObject Bridge;

    /// <summary>
    /// 桥消失多久
    /// </summary>
    [SerializeField] private float BridgeDisappearDuration;

    private CoroutineManager coroutineManager => CoroutineManager.Instance;

    void OnEnable()
    {
        coroutineManager.Run("BeatBridge" + gameObject.GetInstanceID(), BeatBridgeShowAndHide());
    }

    void OnDisable()
    {
        coroutineManager?.Stop("BeatBridge" + gameObject.GetInstanceID());
    }

    IEnumerator BeatBridgeShowAndHide()
    {
        while(true)
        {
            Bridge.SetActive(true);

            for (int i = 0; i < PreludeClips.Length; i++)
            {
                if(BeatPlayAudioSource.clip!= null)
                {
                    BeatPlayAudioSource.Stop();
                }
                BeatPlayAudioSource.clip = PreludeClips[i];
                BeatPlayAudioSource.Play();
                yield return new WaitForSeconds(PreludeInterval);
            }

            BeatPlayAudioSource.Stop();
            BeatPlayAudioSource.clip = FinalBeatClip;
            BeatPlayAudioSource.Play();

            yield return new WaitForSeconds(FinalBeatDelay);

            Bridge.SetActive(false);
            yield return new WaitForSeconds(BridgeDisappearDuration);
        }
    }



#if UNITY_EDITOR
    private void OnValidate()
    {
        if (BeatPlayAudioSource == null)
        {
            BeatPlayAudioSource = GetComponent<AudioSource>();
        }
    }
    #endif
}

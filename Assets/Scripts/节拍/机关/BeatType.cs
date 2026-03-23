using UnityEngine;

[System.Serializable]
public class BeatType
{
    /// <summary>
    /// 节拍检测类型
    /// </summary>
    public BeatCheckType BeatCheckType;
    public AudioClip BeatAudioClip;

    [Header("双节拍检测使用")]
    [Header("双节拍中第一个出现的节拍")]
    public BeatCheckType FirstBeatCheckType;

    [Header("双节拍中第二个节拍音效")]
    public AudioClip SecondBeatAudioClip;

    [Header("双节拍出现的间隔")]
    public float IntervalBetweenTwoBeats;
}

[System.Serializable]
public class MetronomePoint
{
    public PointBeatController PointBeatController;
    public BeatType beatType;
}

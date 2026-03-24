using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Beat Timeline", fileName = "BeatTimeline")]
public class BeatTimelineSO : ScriptableObject
{
    public AudioClip sourceClip;
    public float detectedBpm;
    public float analysisOffset;

    public List<float> beatTimes = new List<float>();
}
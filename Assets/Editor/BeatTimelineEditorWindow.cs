#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BeatTimelineEditorWindow : EditorWindow
{
    private AudioClip clip;

    private float thresholdMultiplier = 1.35f;
    private float absoluteEnergyFloor = 0.0005f;
    private float minBeatInterval = 0.18f;
    private int analysisWindow = 1024;
    private int historyWindowCount = 20;
    private int hopSize = 512;

    private string outputAssetName = "BeatTimeline";

    private List<float> beatTimes = new List<float>();
    private float[] monoSamples;
    private float detectedBpm = 0f;
    private float analysisOffset = 0f;

    [MenuItem("Tools/节拍时间线生成器")]
    public static void Open()
    {
        GetWindow<BeatTimelineEditorWindow>("节拍时间线生成器");
    }

    private void OnGUI()
    {
        clip = (AudioClip)EditorGUILayout.ObjectField("音源", clip, typeof(AudioClip), false);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("阈值越大，检测到的节拍越少", EditorStyles.whiteLabel);
        thresholdMultiplier = EditorGUILayout.Slider("阈值乘数", thresholdMultiplier, 1.0f, 3.0f);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("过滤过小声音，避免误判", EditorStyles.whiteLabel);
        absoluteEnergyFloor = EditorGUILayout.FloatField("最小音量过滤", absoluteEnergyFloor);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("防止一个节拍被检测多次", EditorStyles.whiteLabel);
        minBeatInterval = EditorGUILayout.FloatField("节拍最小间隔（秒）", minBeatInterval);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("越大越平滑，越小越敏感", EditorStyles.whiteLabel);
        analysisWindow = EditorGUILayout.IntField("分析精度", analysisWindow);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("越大越稳定，越小越敏感", EditorStyles.whiteLabel);
        historyWindowCount = EditorGUILayout.IntField("稳定程度", historyWindowCount);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("越小越精细", EditorStyles.whiteLabel);
        hopSize = EditorGUILayout.IntField("分析步长", hopSize);

        EditorGUILayout.Space(10);
        outputAssetName = EditorGUILayout.TextField("保存名称：", outputAssetName);

        EditorGUILayout.Space(8);

        using (new EditorGUI.DisabledScope(clip == null))
        {
            if (GUILayout.Button("Analyze"))
            {
                AnalyzeClip();
            }

            if (GUILayout.Button("Generate SO"))
            {
                SaveAsScriptableObject();
            }
        }

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField($"检测到的节拍数: {beatTimes.Count}");
        if (detectedBpm > 0f)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("每分钟节拍数（节奏速度）", EditorStyles.whiteLabel);
            EditorGUILayout.LabelField($"估算 BPM: {detectedBpm:0.00}");
        }

        DrawPreview();
    }

    private void AnalyzeClip()
    {
        if (clip == null)
        {
            return;
        }

        int frameCount = clip.samples;
        int channels = clip.channels;

        if (frameCount <= 0 || channels <= 0)
        {
            Debug.LogWarning("AudioClip 数据无效。");
            return;
        }

        float[] raw = new float[frameCount * channels];
        clip.GetData(raw, 0);

        monoSamples = new float[frameCount];
        for (int frame = 0; frame < frameCount; frame++)
        {
            float sum = 0f;
            int baseIndex = frame * channels;
            for (int c = 0; c < channels; c++)
            {
                sum += raw[baseIndex + c];
            }
            monoSamples[frame] = sum / channels;
        }

        int window = Mathf.Max(64, analysisWindow);
        int hop = Mathf.Clamp(hopSize, 1, window);

        List<float> results = new List<float>();
        Queue<float> history = new Queue<float>();
        float historySum = 0f;
        float lastBeatTime = -999f;

        for (int start = 0; start + window < frameCount; start += hop)
        {
            float energy = 0f;
            for (int i = 0; i < window; i++)
            {
                float s = monoSamples[start + i];
                energy += s * s;
            }
            energy /= window;

            float avg = history.Count > 0 ? historySum / history.Count : energy;
            float time = start / (float)clip.frequency;

            bool enoughHistory = history.Count >= Mathf.Max(4, historyWindowCount / 2);
            bool isBeat = enoughHistory &&
                          energy > avg * thresholdMultiplier &&
                          energy > absoluteEnergyFloor &&
                          (time - lastBeatTime) >= minBeatInterval;

            if (isBeat)
            {
                results.Add(time);
                lastBeatTime = time;
            }

            history.Enqueue(energy);
            historySum += energy;

            if (history.Count > historyWindowCount)
            {
                historySum -= history.Dequeue();
            }
        }

        beatTimes = results;
        detectedBpm = EstimateBpm(beatTimes);
        analysisOffset = beatTimes.Count > 0 ? beatTimes[0] : 0f;

        Repaint();
        Debug.Log($"分析完成：{beatTimes.Count} 个节拍点，BPM 约 {detectedBpm:0.00}");
    }

    private float EstimateBpm(List<float> beats)
    {
        if (beats == null || beats.Count < 2)
        {
            return 0f;
        }

        float sum = 0f;
        int count = 0;

        for (int i = 1; i < beats.Count; i++)
        {
            float delta = beats[i] - beats[i - 1];
            if (delta > 0.01f)
            {
                sum += delta;
                count++;
            }
        }

        if (count == 0)
        {
            return 0f;
        }

        float avgInterval = sum / count;
        return 60f / avgInterval;
    }

    private void SaveAsScriptableObject()
    {
        if (clip == null)
        {
            return;
        }

        if (beatTimes == null || beatTimes.Count == 0)
        {
            Debug.LogWarning("还没有分析出节拍点。");
            return;
        }

        string path = EditorUtility.SaveFilePanelInProject(
            "Save Beat Timeline",
            string.IsNullOrWhiteSpace(outputAssetName) ? "BeatTimeline" : outputAssetName,
            "asset",
            "Choose where to save the ScriptableObject asset."
        );

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        BeatTimelineSO asset = CreateInstance<BeatTimelineSO>();
        asset.sourceClip = clip;
        asset.detectedBpm = detectedBpm;
        asset.analysisOffset = analysisOffset;
        asset.beatTimes = new List<float>(beatTimes);

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = asset;
        Debug.Log($"已生成 SO：{path}");
    }

    private void DrawPreview()
    {
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

        Rect rect = GUILayoutUtility.GetRect(10, 180, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

        if (monoSamples == null || monoSamples.Length == 0)
        {
            EditorGUI.LabelField(rect, "Analyze 后这里会显示波形和节拍点");
            return;
        }

        Handles.BeginGUI();

        // Waveform
        Handles.color = new Color(0.7f, 0.9f, 1f, 1f);
        int drawPoints = Mathf.Min(2000, monoSamples.Length);
        float step = (monoSamples.Length - 1f) / Mathf.Max(1, drawPoints - 1);
        Vector2? prev = null;

        for (int i = 0; i < drawPoints; i++)
        {
            int sampleIndex = Mathf.Clamp(Mathf.RoundToInt(i * step), 0, monoSamples.Length - 1);
            float x = rect.x + (i / (float)(drawPoints - 1)) * rect.width;
            float y = rect.center.y - Mathf.Clamp(monoSamples[sampleIndex], -1f, 1f) * rect.height * 0.42f;
            Vector2 now = new Vector2(x, y);

            if (prev.HasValue)
            {
                Handles.DrawLine(prev.Value, now);
            }

            prev = now;
        }

        // Beat markers
        Handles.color = Color.red;
        float clipLength = clip != null ? clip.length : 1f;
        foreach (float t in beatTimes)
        {
            float x = rect.x + Mathf.Clamp01(t / clipLength) * rect.width;
            Handles.DrawLine(new Vector2(x, rect.y), new Vector2(x, rect.yMax));
        }

        Handles.EndGUI();
    }
}
#endif
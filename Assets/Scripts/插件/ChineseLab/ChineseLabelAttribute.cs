using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 字段中文标签显示特性
/// 用于将Unity检查器中的字段标签显示为中文名称
/// </summary>
public class ChineseLabelAttribute : PropertyAttribute
{
    public string chineseName;
    
    public ChineseLabelAttribute(string chineseName)
    {
        this.chineseName = chineseName;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ChineseLabelAttribute))]
public class ChineseLabelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var att = attribute as ChineseLabelAttribute;
        
        // 使用中文名称替换原有的标签
        var chineseLabel = new GUIContent(att.chineseName, label.tooltip);
        
        // 绘制属性字段，使用中文标签
        EditorGUI.PropertyField(position, property, chineseLabel, true);
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 返回属性的正确高度
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif
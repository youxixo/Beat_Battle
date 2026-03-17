using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpherePainter : MonoBehaviour
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private Color color = Color.red;
    [SerializeField, Range(12, 120)] private int segments = 60; // 这里的 segments 决定圆环的平滑度

    [SerializeField] private bool isDraw = true;

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!isDraw) return;
        Gizmos.color = color;
        Vector3 center = transform.position;

        // 1. 最简单的办法：直接调用 Unity 内置的线框球
        // Gizmos.DrawWireSphere(center, radius);

        // 2. 如果你想模仿你给出的代码风格，手动画出 XYZ 三个平面的大圆环：
        DrawCircle(center, Vector3.up, radius, segments);    // 水平环 (XZ)
        DrawCircle(center, Vector3.forward, radius, segments); // 垂直环 (XY)
        DrawCircle(center, Vector3.right, radius, segments);   // 侧面环 (YZ)
    }

    private void DrawCircle(Vector3 center, Vector3 normal, float r, int sideCount)
    {
        // 根据法线方向计算旋转量
        Quaternion rotation = Quaternion.LookRotation(normal);
        Vector3 prevPoint = center + rotation * (Vector3.right * r);

        for (int i = 1; i <= sideCount; i++)
        {
            float angle = i * 2 * Mathf.PI / sideCount;
            // 在局部平面计算坐标
            Vector3 localPoint = new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 0);
            // 转换到世界坐标旋转
            Vector3 newPoint = center + rotation * localPoint;

            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
    #endif

    /// <summary>
    /// 获取圆环半径
    /// </summary>
    public float GetRadius => radius;
}

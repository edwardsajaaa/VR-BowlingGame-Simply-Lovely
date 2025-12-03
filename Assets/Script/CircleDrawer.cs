using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    void Start()
    {
        DrawCircle();
    }

    void DrawCircle()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        if (lr == null) return;

        int segments = 50;
        float radius = 2f;

        lr.positionCount = segments + 1;
        lr.useWorldSpace = false;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.loop = true;

        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            lr.SetPosition(i, new Vector3(x, y, 0));
            angle += (360f / segments);
        }
    }
}
using UnityEngine;
using UnityEditor;

public class CircleCreator : EditorWindow
{
    private int segments = 50;
    private float radius = 1f;
    private string objectName = "Circle";

    [MenuItem("GameObject/Create Circle")]
    static void Init()
    {
        CircleCreator window = (CircleCreator)EditorWindow.GetWindow(typeof(CircleCreator));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Circle Creator", EditorStyles.boldLabel);
        
        objectName = EditorGUILayout.TextField("Name", objectName);
        segments = EditorGUILayout.IntField("Segments", segments);
        radius = EditorGUILayout.FloatField("Radius", radius);

        if (GUILayout.Button("Create Circle"))
        {
            CreateCircleObject();
        }
    }

    void CreateCircleObject()
    {
        // Create GameObject
        GameObject circleObj = new GameObject(objectName);
        
        // Add LineRenderer
        LineRenderer lineRenderer = circleObj.AddComponent<LineRenderer>();
        
        // Configure LineRenderer
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.loop = true;
        
        // Create circle points
        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            
            angle += (360f / segments);
        }
        
        // Select the created object
        Selection.activeGameObject = circleObj;
        
        Debug.Log($"Circle '{objectName}' created with {segments} segments and radius {radius}");
    }
}

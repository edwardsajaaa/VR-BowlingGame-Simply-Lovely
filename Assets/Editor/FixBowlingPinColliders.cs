using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public class FixBowlingPinColliders
{
    static FixBowlingPinColliders()
    {
        EditorApplication.delayCall += FixAllPins;
    }

    static void FixAllPins()
    {
        // Find all GameObjects with "Bowling_pin" in name
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int fixedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Bowling_pin"))
            {
                MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
                
                if (meshCollider != null && !meshCollider.convex)
                {
                    meshCollider.convex = true;
                    fixedCount++;
                    Debug.Log($"Fixed MeshCollider on {obj.name} - set to convex");
                }

                // Add Rigidbody if missing
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = obj.AddComponent<Rigidbody>();
                    rb.mass = 1.5f;
                    rb.useGravity = true;
                    Debug.Log($"Added Rigidbody to {obj.name}");
                }

                // Add BowlingPin script if missing
                BowlingPin pinScript = obj.GetComponent<BowlingPin>();
                if (pinScript == null)
                {
                    obj.AddComponent<BowlingPin>();
                    Debug.Log($"Added BowlingPin script to {obj.name}");
                }
            }
        }

        if (fixedCount > 0)
        {
            Debug.Log($"<color=green>✓ Fixed {fixedCount} bowling pins colliders!</color>");
        }
    }
}
#endif

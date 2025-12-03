using UnityEngine;

public class BowlingPin : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool hasFallen = false;
    
    [Header("Pin Settings")]
    public float fallThreshold = 30f;
    public float pinMass = 1.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        rb.mass = pinMass;
        rb.useGravity = true;
        
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        CheckIfFallen();
    }

    void CheckIfFallen()
    {
        float angle = Vector3.Angle(Vector3.up, transform.up);
        
        if (!hasFallen && angle > fallThreshold)
        {
            hasFallen = true;
            if (BowlingGameManager.Instance != null)
            {
                BowlingGameManager.Instance.PinFell();
            }
        }
    }

    public bool HasFallen()
    {
        return hasFallen;
    }

    public void ResetPin()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        hasFallen = false;
    }
}
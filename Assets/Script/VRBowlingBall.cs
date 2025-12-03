using UnityEngine;

public class VRBowlingBall : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 startPosition;
    
    [Header("Ball Settings")]
    public float ballMass = 5f;
    public float throwForce = 500f;
    public float maxThrowForce = 1000f;
    
    [Header("Controls")]
    public KeyCode throwKey = KeyCode.Space;
    public KeyCode resetKey = KeyCode.B;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        rb.mass = ballMass;
        rb.useGravity = true;
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;
        
        startPosition = transform.position;
    }

    void Update()
    {
        // Keyboard controls for testing
        if (Input.GetKeyDown(throwKey))
        {
            ThrowBall();
        }
        
        // Manual reset
        if (Input.GetKeyDown(resetKey))
        {
            ResetBall();
        }
        
        // Auto-reset if ball falls off or goes too far
        if (transform.position.y < -5f || transform.position.z > 25f)
        {
            Invoke("ResetBall", 2f);
        }
    }
    
    void ThrowBall()
    {
        // Simple forward throw
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.forward * throwForce);
        rb.AddForce(Vector3.up * 50f); // Slight upward force
        rb.angularVelocity = Random.insideUnitSphere * 3f;
        
        Debug.Log("Ball thrown with force: " + throwForce);
    }
    
    public void ResetBall()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        Debug.Log("Ball reset to starting position");
    }
}
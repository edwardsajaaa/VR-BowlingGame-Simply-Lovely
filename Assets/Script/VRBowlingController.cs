using UnityEngine;

public class VRBowlingController : MonoBehaviour
{
    [Header("References")]
    public Camera vrCamera;
    public GameObject bowlingBall;
    
    [Header("Throwing Settings")]
    public float throwForce = 800f;
    public float maxThrowForce = 1200f;
    public KeyCode throwKey = KeyCode.Space;
    
    [Header("Mobile Touch")]
    public bool useTouchInput = true;
    
    private bool isThrowing = false;
    private float throwStartTime;
    private Vector3 throwStartPos;

    void Start()
    {
        if (vrCamera == null)
        {
            vrCamera = Camera.main;
        }
        
        if (bowlingBall == null)
        {
            bowlingBall = GameObject.Find("BowlingBall");
        }
        
        // Enable touch input on mobile
        if (Application.isMobilePlatform)
        {
            useTouchInput = true;
        }
    }

    void Update()
    {
        // Desktop: Space key to throw
        if (Input.GetKeyDown(throwKey))
        {
            ThrowBall();
        }
        
        // Mobile: Tap screen to throw
        if (useTouchInput && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                ThrowBall();
            }
        }
        
        // Reset with R key or two-finger tap
        if (Input.GetKeyDown(KeyCode.R) || (Input.touchCount >= 2))
        {
            ResetGame();
        }
    }
    
    void ThrowBall()
    {
        if (bowlingBall == null) return;
        
        Rigidbody rb = bowlingBall.GetComponent<Rigidbody>();
        if (rb == null) return;
        
        // Get camera forward direction
        Vector3 throwDirection = vrCamera.transform.forward;
        
        // Add slight upward angle
        throwDirection.y = Mathf.Max(throwDirection.y, 0.1f);
        throwDirection.Normalize();
        
        // Reset velocity
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        // Apply throw force
        rb.AddForce(throwDirection * throwForce);
        rb.AddTorque(Random.insideUnitSphere * 100f);
        
        Debug.Log($"Ball thrown with force: {throwForce}");
    }
    
    void ResetGame()
    {
        BowlingGameManager manager = FindObjectOfType<BowlingGameManager>();
        if (manager != null)
        {
            // Call reset via SendMessage
            manager.SendMessage("ResetGame", SendMessageOptions.DontRequireReceiver);
        }
        
        // Reset ball manually if manager not found
        if (bowlingBall != null)
        {
            VRBowlingBall ballScript = bowlingBall.GetComponent<VRBowlingBall>();
            if (ballScript != null)
            {
                ballScript.ResetBall();
            }
        }
        
        Debug.Log("Game reset requested");
    }
}
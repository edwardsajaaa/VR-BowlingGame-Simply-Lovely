using UnityEngine;

public class VRCameraController : MonoBehaviour
{
    [Header("VR Settings")]
    public bool enableGyroscope = true;
    public float mouseSensitivity = 2f;
    
    private Gyroscope gyro;
    private bool gyroSupported;
    private Quaternion baseRotation;
    
    void Start()
    {
        // Try to enable gyroscope
        gyroSupported = SystemInfo.supportsGyroscope;
        
        if (gyroSupported && enableGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            baseRotation = Quaternion.Euler(90, 0, 0);
            Debug.Log("Gyroscope enabled for VR!");
        }
        else
        {
            Debug.Log("Gyroscope not supported or disabled. Using mouse control.");
        }
        
        // Lock cursor for desktop testing
        if (!Application.isMobilePlatform)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    void Update()
    {
        if (gyroSupported && enableGyroscope && Application.isMobilePlatform)
        {
            // Gyroscope control for mobile VR
            Quaternion gyroRotation = gyro.attitude;
            transform.localRotation = baseRotation * Quaternion.Euler(0, 0, 180) * gyroRotation;
        }
        else
        {
            // Mouse control for desktop testing
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            
            Vector3 currentRotation = transform.localEulerAngles;
            currentRotation.y += mouseX;
            currentRotation.x -= mouseY;
            
            // Clamp vertical rotation
            if (currentRotation.x > 180)
                currentRotation.x -= 360;
            currentRotation.x = Mathf.Clamp(currentRotation.x, -80, 80);
            
            transform.localEulerAngles = currentRotation;
        }
        
        // Unlock cursor with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
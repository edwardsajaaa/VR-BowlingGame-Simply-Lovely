using UnityEngine;

public class VRPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 100f;
    
    [Header("Gyroscope Sensitivity")]
    public float forwardTiltThreshold = 20f; // Derajat kemiringan untuk gerak maju
    public float backwardTiltThreshold = -20f; // Derajat kemiringan untuk gerak mundur
    public float sideTiltThreshold = 15f; // Derajat kemiringan untuk gerak samping
    
    private CharacterController characterController;
    private Quaternion initialRotation;
    private bool gyroEnabled = false;
    
    void Start()
    {
        // Setup Character Controller
        characterController = gameObject.AddComponent<CharacterController>();
        characterController.height = 1.8f;
        characterController.radius = 0.3f;
        characterController.center = new Vector3(0, 0.9f, 0);
        
        // Enable gyroscope
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            gyroEnabled = true;
            // Kalibrasi rotasi awal
            initialRotation = Quaternion.Euler(90f, 0f, 0f);
            Debug.Log("Gyroscope enabled for player movement");
        }
        else
        {
            Debug.LogWarning("Gyroscope not supported on this device");
        }
    }
    
    void Update()
    {
        Vector3 movement = Vector3.zero;
        
        if (gyroEnabled)
        {
            // Baca gyroscope
            Quaternion gyroAttitude = Input.gyro.attitude;
            Quaternion adjustedRotation = initialRotation * gyroAttitude;
            
            // Konversi ke Euler angles
            Vector3 euler = adjustedRotation.eulerAngles;
            
            // Normalisasi sudut ke range -180 hingga 180
            float pitch = NormalizeAngle(euler.x); // Kemiringan depan/belakang
            float roll = NormalizeAngle(euler.z);  // Kemiringan kiri/kanan
            
            // Gerak maju/mundur berdasarkan pitch (miringkan HP ke depan/belakang)
            if (pitch > forwardTiltThreshold)
            {
                movement.z = 1f; // Maju
            }
            else if (pitch < backwardTiltThreshold)
            {
                movement.z = -1f; // Mundur
            }
            
            // Gerak samping berdasarkan roll (miringkan HP ke kiri/kanan)
            if (roll > sideTiltThreshold)
            {
                movement.x = 1f; // Kanan
            }
            else if (roll < -sideTiltThreshold)
            {
                movement.x = -1f; // Kiri
            }
            
            // Debug info
            if (Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log($"Gyro - Pitch: {pitch:F1}°, Roll: {roll:F1}° | Movement: {movement}");
            }
        }
        else
        {
            // Fallback keyboard control untuk testing di editor
            movement.x = Input.GetAxis("Horizontal");
            movement.z = Input.GetAxis("Vertical");
        }
        
        // Terapkan gerakan relatif terhadap arah player
        Vector3 move = transform.TransformDirection(movement) * moveSpeed * Time.deltaTime;
        
        // Tambahkan gravity
        move.y = -9.81f * Time.deltaTime;
        
        // Gerakkan character
        characterController.Move(move);
        
        // Rotasi dengan mouse (untuk testing) atau tombol A/D
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
        
        float mouseX = Input.GetAxis("Mouse X");
        if (Mathf.Abs(mouseX) > 0.01f && Input.GetMouseButton(1))
        {
            transform.Rotate(0, mouseX * rotationSpeed * Time.deltaTime, 0);
        }
    }
    
    // Normalisasi sudut dari 0-360 ke -180 hingga 180
    float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }
    
    void OnDrawGizmos()
    {
        // Visualisasi arah movement
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 1f);
    }
}

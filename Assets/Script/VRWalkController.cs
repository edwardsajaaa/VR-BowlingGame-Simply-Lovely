using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class VRWalkController : MonoBehaviour
{
    [Header("Pengaturan Gerak")]
    public float speed = 3.0f; // Kecepatan jalan
    
    [Header("Pengaturan Sudut (Derajat)")]
    [Tooltip("Sudut minimal menunduk untuk mulai jalan")]
    public float startWalkAngle = 15.0f; 
    
    [Tooltip("Sudut maksimal (jika nunduk terlalu parah, berhenti)")]
    public float stopWalkAngle = 70.0f;

    private CharacterController controller;
    private Transform cameraTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // Mengambil referensi Kamera Utama (Mata VR)
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // 1. Ambil rotasi X kamera (Pitch/Menunduk)
        float xAngle = cameraTransform.eulerAngles.x;

        // 2. Normalisasi sudut (Unity kadang membaca 0-360)
        // Kita ingin range standar untuk nunduk (0 sampai 90)
        if (xAngle > 180) xAngle -= 360; 

        // 3. Cek apakah sudut memenuhi syarat untuk jalan
        // Logika: Jika sudut lebih besar dari batas mulai DAN lebih kecil dari batas henti
        bool isLookingDown = (xAngle >= startWalkAngle && xAngle <= stopWalkAngle);

        if (isLookingDown)
        {
            MoveForward();
        }
    }

    void MoveForward()
    {
        // Ambil arah depan kamera
        Vector3 forward = cameraTransform.forward;

        // Hapus komponen Y (agar tidak terbang ke bawah/atas saat nunduk)
        forward.y = 0;
        forward.Normalize();

        // Gerakkan Character Controller
        controller.SimpleMove(forward * speed);
    }
}
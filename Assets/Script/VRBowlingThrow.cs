using UnityEngine;

public class VRBowlingThrow : MonoBehaviour
{
    [Header("Setup Komponen")]
    [Tooltip("Masukkan Prefab Bola Bowling di sini")]
    public GameObject ballPrefab;

    [Tooltip("Masukkan Transform (Empty Object) posisi tangan/muncul bola")]
    public Transform handPosition;

    [Header("Setting Kekuatan")]
    public float minForce = 5f;        // Kekuatan minimal (tap cepat)
    public float maxForce = 35f;       // Kekuatan maksimal (tahan lama)
    public float chargeSpeed = 20f;    // Seberapa cepat power terisi

    [Header("Debug Info (Hanya Baca)")]
    public float currentPower;         // Untuk melihat power saat ini di Inspector
    public bool isCharging = false;

    private PinManager pinManager;

    private void Start()
    {
        // Cari PinManager di scene
        pinManager = FindObjectOfType<PinManager>();
    }

    private void Update()
    {
        // --- LOGIKA INPUT (Touch HP / Klik Mouse) ---

        // 1. Mulai tekan (Start Charging)
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            currentPower = minForce; // Reset power ke minimal
        }

        // 2. Sedang menahan (Charging Power)
        if (Input.GetMouseButton(0) && isCharging)
        {
            currentPower += chargeSpeed * Time.deltaTime;

            // Batasi power agar tidak melebihi batas maksimal
            if (currentPower > maxForce)
            {
                currentPower = maxForce;
            }
        }

        // 3. Lepas jari (Lempar!)
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            ThrowBall();
            isCharging = false; // Reset status
        }
    }

    private void ThrowBall()
    {
        // Cek dulu apakah komponen sudah dipasang agar tidak error
        if (ballPrefab == null || handPosition == null)
        {
            Debug.LogError("Setup Belum Lengkap! Masukkan Ball Prefab dan Hand Position di Inspector.");
            return;
        }

        // 1. Munculkan Bola (Instantiate)
        GameObject ball = Instantiate(ballPrefab, handPosition.position, handPosition.rotation);
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // --- SETTING FISIKA (RAHASIA AGAR REALISTIS) ---

            // PENTING: Melepas batas kecepatan putar Unity (default cuma 7, kita ubah jadi 150)
            rb.maxAngularVelocity = 150f;

            // Kurangi gesekan udara & putaran agar bola tidak cepat berhenti
            rb.drag = 0.05f;          
            rb.angularDrag = 0.05f;
            rb.useGravity = true;

            // --- KALKULASI ARAH & TENAGA ---

            // Ambil arah depan kamera (ke mana pemain melihat)
            Vector3 forwardDir = Camera.main.transform.forward;

            // Tambahkan gaya dorong (Force)
            rb.AddForce(forwardDir * currentPower, ForceMode.Impulse);

            // --- RAHASIA BOLA MENGGELINDING (TOP SPIN) ---
            // Kita beri gaya putar (Torque) ke depan agar bola langsung 'menggigit' lantai
            // Sumbu putar adalah kanan kamera (Right), agar bola berputar ke depan
            rb.AddTorque(Camera.main.transform.right * currentPower, ForceMode.Impulse);

            // Trigger deteksi pin setelah bola dilempar
            if (pinManager != null)
            {
                pinManager.StartDetection();
            }
        }
    }
}
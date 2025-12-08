using UnityEngine;

public class PinManager : MonoBehaviour
{
    [Header("Pin Settings")]
    public GameObject[] bowlingPins;     // Array semua pin (10 pin)
    public Transform pinSpawnParent;     // Parent object untuk organize pin
    public GameObject pinPrefab;         // Prefab pin jika ingin auto-generate

    [Header("Detection Settings")]
    [Tooltip("Sudut minimal pin dianggap jatuh (derajat)")]
    public float fallAngleThreshold = 45f;
    
    [Tooltip("Delay deteksi setelah bola dilempar (detik)")]
    public float detectionDelay = 3f;

    private Vector3[] originalPositions;
    private Quaternion[] originalRotations;
    private BowlingScoreManager scoreManager;
    private bool isDetecting = false;

    void Start()
    {
        scoreManager = FindObjectOfType<BowlingScoreManager>();
        
        // Jika pin belum diassign, cari otomatis by tag
        if (bowlingPins == null || bowlingPins.Length == 0)
        {
            bowlingPins = GameObject.FindGameObjectsWithTag("BowlingPin");
        }

        // Simpan posisi awal semua pin
        SaveOriginalPositions();
    }

    void SaveOriginalPositions()
    {
        if (bowlingPins == null || bowlingPins.Length == 0)
        {
            Debug.LogWarning("Tidak ada bowling pin yang ditemukan!");
            return;
        }

        originalPositions = new Vector3[bowlingPins.Length];
        originalRotations = new Quaternion[bowlingPins.Length];

        for (int i = 0; i < bowlingPins.Length; i++)
        {
            if (bowlingPins[i] != null)
            {
                originalPositions[i] = bowlingPins[i].transform.position;
                originalRotations[i] = bowlingPins[i].transform.rotation;
            }
        }
    }

    // Dipanggil saat bola dilempar
    public void StartDetection()
    {
        if (!isDetecting)
        {
            Invoke(nameof(DetectFallenPins), detectionDelay);
            isDetecting = true;
        }
    }

    void DetectFallenPins()
    {
        int fallenPins = CountFallenPins();
        
        Debug.Log($"Pin yang jatuh: {fallenPins}");

        // Kirim hasil ke ScoreManager
        if (scoreManager != null)
        {
            scoreManager.RegisterPinsKnocked(fallenPins);
        }

        isDetecting = false;
    }

    int CountFallenPins()
    {
        int count = 0;

        foreach (GameObject pin in bowlingPins)
        {
            if (pin != null && IsPinFallen(pin))
            {
                count++;
            }
        }

        return count;
    }

    bool IsPinFallen(GameObject pin)
    {
        // Cek sudut rotasi pin
        // Jika pin miring lebih dari threshold, dianggap jatuh
        Vector3 upDirection = pin.transform.up;
        float angle = Vector3.Angle(upDirection, Vector3.up);

        return angle > fallAngleThreshold;
    }

    public void ResetAllPins()
    {
        for (int i = 0; i < bowlingPins.Length; i++)
        {
            if (bowlingPins[i] != null)
            {
                // Reset posisi dan rotasi
                bowlingPins[i].transform.position = originalPositions[i];
                bowlingPins[i].transform.rotation = originalRotations[i];

                // Reset velocity rigidbody
                Rigidbody rb = bowlingPins[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        Debug.Log("Semua pin telah direset!");
    }

    // Reset hanya pin yang jatuh (untuk lemparan kedua)
    public void ResetFallenPins()
    {
        for (int i = 0; i < bowlingPins.Length; i++)
        {
            if (bowlingPins[i] != null && IsPinFallen(bowlingPins[i]))
            {
                // Hapus atau hide pin yang jatuh
                bowlingPins[i].SetActive(false);
            }
        }
    }

    // Generate pin arrangement (formasi segitiga)
    public void GeneratePinFormation(Vector3 startPosition, float spacing = 0.3f)
    {
        bowlingPins = new GameObject[10];
        int pinIndex = 0;

        // Formasi bowling (1-2-3-4 row)
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col <= row; col++)
            {
                Vector3 position = startPosition +
                    new Vector3(col * spacing - (row * spacing / 2f), 0, -row * spacing * 0.866f);

                GameObject pin = Instantiate(pinPrefab, position, Quaternion.identity);
                pin.tag = "BowlingPin";
                pin.name = $"Pin_{pinIndex + 1}";

                if (pinSpawnParent != null)
                {
                    pin.transform.SetParent(pinSpawnParent);
                }

                bowlingPins[pinIndex] = pin;
                pinIndex++;
            }
        }

        SaveOriginalPositions();
        Debug.Log("10 bowling pins telah di-generate!");
    }
}

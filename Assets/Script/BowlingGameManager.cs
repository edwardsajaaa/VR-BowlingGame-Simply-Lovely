using UnityEngine;
using UnityEngine.UI;

public class BowlingGameManager : MonoBehaviour
{
    public static BowlingGameManager Instance;
    
    [Header("Game Settings")]
    public int totalPins = 10;
    public float resetDelay = 3f;
    
    [Header("References")]
    public GameObject[] pins;
    public GameObject bowlingBall;
    public Text scoreText;
    
    private int currentScore = 0;
    private int fallenPinsCount = 0;
    private bool isResetting = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        FindAllPins();
        UpdateScoreUI();
    }

    void FindAllPins()
    {
        BowlingPin[] pinScripts = FindObjectsOfType<BowlingPin>();
        pins = new GameObject[pinScripts.Length];
        
        for (int i = 0; i < pinScripts.Length; i++)
        {
            pins[i] = pinScripts[i].gameObject;
        }
        
        Debug.Log($"Found {pins.Length} bowling pins");
    }

    public void PinFell()
    {
        fallenPinsCount++;
        currentScore += 10;
        UpdateScoreUI();
        
        Debug.Log($"Pin fell! Total fallen: {fallenPinsCount}, Score: {currentScore}");
        
        if (fallenPinsCount >= totalPins && !isResetting)
        {
            Invoke("ResetGame", resetDelay);
            isResetting = true;
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}\nPins: {fallenPinsCount}/{totalPins}";
        }
    }

    void ResetGame()
    {
        Debug.Log("Resetting game...");
        
        // Reset all pins
        foreach (GameObject pin in pins)
        {
            BowlingPin pinScript = pin.GetComponent<BowlingPin>();
            if (pinScript != null)
            {
                pinScript.ResetPin();
            }
        }
        
        // Reset ball
        if (bowlingBall != null)
        {
            VRBowlingBall ballScript = bowlingBall.GetComponent<VRBowlingBall>();
            if (ballScript != null)
            {
                ballScript.SendMessage("ResetBall");
            }
        }
        
        fallenPinsCount = 0;
        isResetting = false;
        
        Debug.Log("Game reset complete!");
    }

    void Update()
    {
        // Manual reset with R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }
}
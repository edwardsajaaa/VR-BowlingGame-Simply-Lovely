using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BowlingScoreManager : MonoBehaviour
{
    [Header("UI References")]
    public Text scoreText;           // Text untuk menampilkan skor total
    public Text frameText;           // Text untuk menampilkan frame saat ini
    public Text statusText;          // Text untuk status (Strike/Spare/Miss)
    public Text[] frameScoreTexts;   // Array Text untuk menampilkan skor tiap frame (10 frame)

    [Header("Game Settings")]
    public int totalPins = 10;       // Total pin dalam bowling
    public int maxFrames = 10;       // Total frame dalam satu game
    public float resetDelay = 3f;    // Delay sebelum reset pin

    [Header("Target Settings")]
    [Tooltip("Skor minimum untuk menang")]
    public int targetScore = 100;
    public Text targetScoreText;     // Text untuk menampilkan target

    [Header("Sound Effects (Optional)")]
    public AudioClip strikeSound;
    public AudioClip spareSound;
    public AudioClip gutterSound;
    private AudioSource audioSource;

    // Sistem Frame Bowling
    private int currentFrame = 1;
    private int currentThrow = 1;    // Lemparan ke-1 atau ke-2 dalam frame
    private int totalScore = 0;
    
    // Menyimpan data tiap frame
    private List<FrameData> frames = new List<FrameData>();
    
    // Game State
    private bool isGameOver = false;
    private int pinsKnockedThisThrow = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        InitializeGame();
    }

    void InitializeGame()
    {
        currentFrame = 1;
        currentThrow = 1;
        totalScore = 0;
        isGameOver = false;
        frames.Clear();

        // Inisialisasi 10 frame
        for (int i = 0; i < maxFrames; i++)
        {
            frames.Add(new FrameData());
        }

        UpdateUI();
    }

    // Dipanggil dari script lain saat pin jatuh
    public void RegisterPinsKnocked(int pinsKnocked)
    {
        if (isGameOver) return;

        pinsKnockedThisThrow = pinsKnocked;
        ProcessThrow(pinsKnocked);
    }

    void ProcessThrow(int pinsKnocked)
    {
        FrameData currentFrameData = frames[currentFrame - 1];

        // Frame 10 (Frame terakhir) punya aturan khusus
        if (currentFrame == maxFrames)
        {
            ProcessFrame10(pinsKnocked);
            return;
        }

        // Frame 1-9 (Normal)
        if (currentThrow == 1)
        {
            // Lemparan pertama
            currentFrameData.firstThrow = pinsKnocked;

            if (pinsKnocked == totalPins)
            {
                // STRIKE!
                currentFrameData.isStrike = true;
                ShowStatus("STRIKE!", Color.yellow);
                PlaySound(strikeSound);
                
                Invoke(nameof(NextFrame), resetDelay);
            }
            else
            {
                // Belum strike, lanjut ke lemparan kedua
                currentThrow = 2;
                ShowStatus($"Pins: {pinsKnocked}/{totalPins}", Color.white);
                
                // Reset pin yang masih berdiri
                Invoke(nameof(PrepareSecondThrow), resetDelay);
            }
        }
        else if (currentThrow == 2)
        {
            // Lemparan kedua
            currentFrameData.secondThrow = pinsKnocked;
            int totalThisFrame = currentFrameData.firstThrow + currentFrameData.secondThrow;

            if (totalThisFrame == totalPins)
            {
                // SPARE!
                currentFrameData.isSpare = true;
                ShowStatus("SPARE!", Color.green);
                PlaySound(spareSound);
            }
            else if (totalThisFrame == 0)
            {
                // GUTTER (Tidak kena sama sekali)
                ShowStatus("Gutter Ball!", Color.red);
                PlaySound(gutterSound);
            }
            else
            {
                ShowStatus($"Total: {totalThisFrame} pins", Color.white);
            }

            Invoke(nameof(NextFrame), resetDelay);
        }

        UpdateUI();
    }

    void ProcessFrame10(int pinsKnocked)
    {
        FrameData frame10 = frames[9];

        if (currentThrow == 1)
        {
            frame10.firstThrow = pinsKnocked;
            
            if (pinsKnocked == totalPins)
            {
                frame10.isStrike = true;
                ShowStatus("STRIKE! (Bonus throw)", Color.yellow);
                PlaySound(strikeSound);
                currentThrow = 2;
                Invoke(nameof(ResetPinsForBonus), resetDelay);
            }
            else
            {
                currentThrow = 2;
                ShowStatus($"Pins: {pinsKnocked}/{totalPins}", Color.white);
                Invoke(nameof(PrepareSecondThrow), resetDelay);
            }
        }
        else if (currentThrow == 2)
        {
            frame10.secondThrow = pinsKnocked;
            int total = frame10.firstThrow + frame10.secondThrow;

            if (frame10.isStrike || total == totalPins)
            {
                // Dapat bonus throw ketiga
                if (total == totalPins && !frame10.isStrike)
                {
                    frame10.isSpare = true;
                    ShowStatus("SPARE! (Bonus throw)", Color.green);
                    PlaySound(spareSound);
                }
                
                currentThrow = 3;
                Invoke(nameof(ResetPinsForBonus), resetDelay);
            }
            else
            {
                ShowStatus($"Frame 10 Total: {total}", Color.white);
                Invoke(nameof(EndGame), resetDelay);
            }
        }
        else if (currentThrow == 3)
        {
            frame10.thirdThrow = pinsKnocked;
            ShowStatus("Game Complete!", Color.cyan);
            Invoke(nameof(EndGame), resetDelay);
        }

        UpdateUI();
    }

    void NextFrame()
    {
        currentFrame++;
        currentThrow = 1;

        if (currentFrame > maxFrames)
        {
            EndGame();
        }
        else
        {
            ResetPins();
            UpdateUI();
        }
    }

    void PrepareSecondThrow()
    {
        // Panggil fungsi untuk membiarkan pin yang berdiri tetap berdiri
        // Anda perlu implementasi di PinManager
        UpdateUI();
    }

    void ResetPinsForBonus()
    {
        ResetPins();
        UpdateUI();
    }

    void ResetPins()
    {
        // Panggil PinManager untuk reset semua pin
        PinManager pinManager = FindObjectOfType<PinManager>();
        if (pinManager != null)
        {
            pinManager.ResetAllPins();
        }
    }

    void EndGame()
    {
        isGameOver = true;
        CalculateFinalScore();
        
        if (totalScore >= targetScore)
        {
            ShowStatus($"YOU WIN! Score: {totalScore}", Color.green);
        }
        else
        {
            ShowStatus($"Game Over. Score: {totalScore} (Target: {targetScore})", Color.red);
        }

        // Restart setelah 5 detik
        Invoke(nameof(RestartGame), 5f);
    }

    void CalculateFinalScore()
    {
        totalScore = 0;

        for (int i = 0; i < maxFrames; i++)
        {
            FrameData frame = frames[i];

            if (i < 9) // Frame 1-9
            {
                if (frame.isStrike)
                {
                    // Strike: 10 + 2 lemparan berikutnya
                    frame.score = 10;
                    
                    // Bonus dari frame berikutnya
                    FrameData nextFrame = frames[i + 1];
                    frame.score += nextFrame.firstThrow;
                    
                    if (nextFrame.isStrike && i < 8)
                    {
                        // Jika next frame juga strike, ambil dari frame setelahnya
                        frame.score += frames[i + 2].firstThrow;
                    }
                    else
                    {
                        frame.score += nextFrame.secondThrow;
                    }
                }
                else if (frame.isSpare)
                {
                    // Spare: 10 + 1 lemparan berikutnya
                    frame.score = 10 + frames[i + 1].firstThrow;
                }
                else
                {
                    // Normal
                    frame.score = frame.firstThrow + frame.secondThrow;
                }
            }
            else // Frame 10
            {
                frame.score = frame.firstThrow + frame.secondThrow + frame.thirdThrow;
            }

            totalScore += frame.score;
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            CalculateFinalScore();
            scoreText.text = $"Score: {totalScore}";
        }

        if (frameText != null)
        {
            frameText.text = $"Frame: {currentFrame}/{maxFrames}";
        }

        if (targetScoreText != null)
        {
            targetScoreText.text = $"Target: {targetScore}";
        }

        // Update individual frame scores
        if (frameScoreTexts != null && frameScoreTexts.Length >= maxFrames)
        {
            for (int i = 0; i < maxFrames; i++)
            {
                if (frameScoreTexts[i] != null)
                {
                    FrameData frame = frames[i];
                    string frameDisplay = "";

                    if (i < currentFrame - 1 || (i == currentFrame - 1 && currentThrow > 1))
                    {
                        if (frame.isStrike)
                            frameDisplay = "X";
                        else if (frame.isSpare)
                            frameDisplay = $"{frame.firstThrow} /";
                        else
                            frameDisplay = $"{frame.firstThrow} {frame.secondThrow}";
                        
                        frameDisplay += $"\n({frame.score})";
                    }

                    frameScoreTexts[i].text = frameDisplay;
                }
            }
        }
    }

    void ShowStatus(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
        }
        
        Debug.Log(message);
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void RestartGame()
    {
        InitializeGame();
        ResetPins();
    }

    // Public method untuk manual restart
    public void ManualRestart()
    {
        RestartGame();
    }
}

// Class untuk menyimpan data per frame
[System.Serializable]
public class FrameData
{
    public int firstThrow = 0;
    public int secondThrow = 0;
    public int thirdThrow = 0;  // Hanya untuk frame 10
    public bool isStrike = false;
    public bool isSpare = false;
    public int score = 0;
}

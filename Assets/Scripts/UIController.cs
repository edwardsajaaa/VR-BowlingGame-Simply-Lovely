using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace VRBowling.Scripts
{
    /// <summary>
    /// Simple UI controller for in-game menus and settings.
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject settingsPanel;
        
        [Header("Game Over UI")]
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        
        [Header("References")]
        [SerializeField] private GameManager gameManager;
        
        [Header("Events")]
        public UnityEvent OnGameStart;
        public UnityEvent OnGamePause;
        public UnityEvent OnGameResume;
        public UnityEvent OnReturnToMenu;
        
        private int highScore;
        private const string HIGH_SCORE_KEY = "BowlingHighScore";
        
        private void Start()
        {
            LoadHighScore();
            ShowMainMenu();
        }
        
        public void ShowMainMenu()
        {
            HideAllPanels();
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
            }
        }
        
        public void ShowPauseMenu()
        {
            HideAllPanels();
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
                Time.timeScale = 0f;
            }
            OnGamePause?.Invoke();
        }
        
        public void ShowGameOver(int finalScore)
        {
            HideAllPanels();
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                
                if (finalScoreText != null)
                {
                    finalScoreText.text = $"Final Score: {finalScore}";
                }
                
                // Check for high score
                if (finalScore > highScore)
                {
                    highScore = finalScore;
                    SaveHighScore();
                }
                
                if (highScoreText != null)
                {
                    highScoreText.text = $"High Score: {highScore}";
                }
            }
        }
        
        public void ShowSettings()
        {
            HideAllPanels();
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }
        }
        
        public void HideAllPanels()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }
        
        public void StartGame()
        {
            HideAllPanels();
            Time.timeScale = 1f;
            
            if (gameManager != null)
            {
                gameManager.NewGame();
            }
            
            OnGameStart?.Invoke();
        }
        
        public void ResumeGame()
        {
            HideAllPanels();
            Time.timeScale = 1f;
            OnGameResume?.Invoke();
        }
        
        public void RestartGame()
        {
            HideAllPanels();
            Time.timeScale = 1f;
            
            if (gameManager != null)
            {
                gameManager.NewGame();
            }
        }
        
        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            ShowMainMenu();
            OnReturnToMenu?.Invoke();
        }
        
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        
        private void LoadHighScore()
        {
            highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        }
        
        private void SaveHighScore()
        {
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }
        
        public void ResetHighScore()
        {
            highScore = 0;
            SaveHighScore();
            if (highScoreText != null)
            {
                highScoreText.text = $"High Score: {highScore}";
            }
        }
    }
}

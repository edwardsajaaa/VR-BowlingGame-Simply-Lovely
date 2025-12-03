using UnityEngine;
using TMPro;

namespace VRBowling.Scripts
{
    /// <summary>
    /// Manages the score display UI for the bowling game.
    /// </summary>
    public class ScoreDisplay : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI totalScoreText;
        [SerializeField] private TextMeshProUGUI currentFrameText;
        [SerializeField] private TextMeshProUGUI[] frameScoreTexts = new TextMeshProUGUI[10];
        [SerializeField] private TextMeshProUGUI messageText;
        
        [Header("Display Settings")]
        [SerializeField] private float messageDisplayTime = 3f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private Color strikeColor = Color.green;
        [SerializeField] private Color spareColor = Color.cyan;
        
        private float messageTimer;
        
        private void Update()
        {
            // Clear message after display time
            if (messageTimer > 0)
            {
                messageTimer -= Time.deltaTime;
                if (messageTimer <= 0 && messageText != null)
                {
                    messageText.text = "";
                }
            }
        }
        
        /// <summary>
        /// Update the score display with current game state.
        /// </summary>
        public void UpdateScore(int[] frameScores, int totalScore, int currentFrame)
        {
            // Update total score
            if (totalScoreText != null)
            {
                totalScoreText.text = $"Total: {totalScore}";
            }
            
            // Update current frame
            if (currentFrameText != null)
            {
                currentFrameText.text = $"Frame: {currentFrame + 1}";
            }
            
            // Update individual frame scores
            for (int i = 0; i < frameScoreTexts.Length && i < frameScores.Length; i++)
            {
                if (frameScoreTexts[i] != null)
                {
                    frameScoreTexts[i].text = frameScores[i].ToString();
                    
                    // Highlight current frame
                    if (i == currentFrame)
                    {
                        frameScoreTexts[i].color = highlightColor;
                    }
                    else
                    {
                        frameScoreTexts[i].color = normalColor;
                    }
                }
            }
        }
        
        /// <summary>
        /// Display a message (Strike!, Spare!, etc.)
        /// </summary>
        public void ShowMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
                
                // Set color based on message type
                if (message.Contains("Strike"))
                {
                    messageText.color = strikeColor;
                }
                else if (message.Contains("Spare"))
                {
                    messageText.color = spareColor;
                }
                else
                {
                    messageText.color = normalColor;
                }
                
                messageTimer = messageDisplayTime;
            }
        }
        
        /// <summary>
        /// Clear all score displays.
        /// </summary>
        public void ClearDisplay()
        {
            if (totalScoreText != null)
            {
                totalScoreText.text = "Total: 0";
            }
            
            if (currentFrameText != null)
            {
                currentFrameText.text = "Frame: 1";
            }
            
            foreach (var frameText in frameScoreTexts)
            {
                if (frameText != null)
                {
                    frameText.text = "0";
                    frameText.color = normalColor;
                }
            }
            
            if (messageText != null)
            {
                messageText.text = "";
            }
        }
        
        /// <summary>
        /// Display game over message.
        /// </summary>
        public void ShowGameOver(int finalScore)
        {
            if (messageText != null)
            {
                messageText.text = $"Game Over!\nFinal Score: {finalScore}";
                messageText.color = highlightColor;
            }
        }
    }
}

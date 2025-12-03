using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace VRBowling.Scripts
{
    /// <summary>
    /// Manages the bowling game flow, scoring, and state.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game References")]
        [SerializeField] private BowlingLane bowlingLane;
        [SerializeField] private BowlingBall bowlingBall;
        [SerializeField] private ScoreDisplay scoreDisplay;
        
        [Header("Game Settings")]
        [SerializeField] private float waitTimeAfterThrow = 3f;
        [SerializeField] private float pinResetDelay = 2f;
        
        [Header("Audio")]
        [SerializeField] private AudioClip strikeSound;
        [SerializeField] private AudioClip spareSound;
        [SerializeField] private AudioClip gutterBallSound;
        
        private AudioSource audioSource;
        
        // Scoring
        private int[] frameScores = new int[10];
        private int[] throwScores = new int[21]; // Maximum 21 throws in a game
        private int currentFrame = 0;
        private int currentThrow = 0;
        private int totalScore = 0;
        private bool isFirstThrowInFrame = true;
        private bool gameOver = false;
        
        // Game state
        public enum GameState
        {
            WaitingForThrow,
            BallInMotion,
            CalculatingScore,
            ResettingPins,
            GameOver
        }
        
        private GameState currentState = GameState.WaitingForThrow;
        
        public int CurrentFrame => currentFrame;
        public int TotalScore => totalScore;
        public bool IsGameOver => gameOver;
        public GameState CurrentGameState => currentState;
        
        // Events
        public UnityEvent<int, int> OnScoreUpdated; // frame, score
        public UnityEvent<string> OnFrameResult; // "Strike!", "Spare!", etc.
        public UnityEvent OnGameOver;
        public UnityEvent OnGameReset;
        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        private void Start()
        {
            InitializeGame();
        }
        
        private void OnEnable()
        {
            if (bowlingBall != null)
            {
                bowlingBall.OnBallThrown += HandleBallThrown;
                bowlingBall.OnBallStopped += HandleBallStopped;
            }
        }
        
        private void OnDisable()
        {
            if (bowlingBall != null)
            {
                bowlingBall.OnBallThrown -= HandleBallThrown;
                bowlingBall.OnBallStopped -= HandleBallStopped;
            }
        }
        
        /// <summary>
        /// Initialize or reset the game.
        /// </summary>
        public void InitializeGame()
        {
            // Reset all scores
            for (int i = 0; i < frameScores.Length; i++)
            {
                frameScores[i] = 0;
            }
            
            for (int i = 0; i < throwScores.Length; i++)
            {
                throwScores[i] = 0;
            }
            
            currentFrame = 0;
            currentThrow = 0;
            totalScore = 0;
            isFirstThrowInFrame = true;
            gameOver = false;
            currentState = GameState.WaitingForThrow;
            
            // Reset pins
            if (bowlingLane != null)
            {
                bowlingLane.SetupPins();
            }
            
            // Reset ball
            if (bowlingBall != null)
            {
                bowlingBall.ResetBall();
            }
            
            // Update display
            UpdateScoreDisplay();
            OnGameReset?.Invoke();
        }
        
        private void HandleBallThrown(BowlingBall ball)
        {
            currentState = GameState.BallInMotion;
        }
        
        private void HandleBallStopped(BowlingBall ball)
        {
            StartCoroutine(ProcessThrowResult());
        }
        
        private IEnumerator ProcessThrowResult()
        {
            currentState = GameState.CalculatingScore;
            
            // Wait for pins to settle
            yield return new WaitForSeconds(waitTimeAfterThrow);
            
            // Count knocked down pins
            int pinsKnockedDown = bowlingLane.GetKnockedDownPinCount();
            int pinsKnockedThisThrow = isFirstThrowInFrame ? 
                pinsKnockedDown : 
                pinsKnockedDown - throwScores[currentThrow - 1];
            
            // Record the throw
            throwScores[currentThrow] = pinsKnockedThisThrow;
            
            // Check for special results
            bool isStrike = isFirstThrowInFrame && pinsKnockedDown == 10;
            bool isSpare = !isFirstThrowInFrame && pinsKnockedDown == 10;
            bool isGutterBall = bowlingBall.IsInGutter && pinsKnockedThisThrow == 0;
            
            // Play appropriate sound
            if (isStrike)
            {
                PlaySound(strikeSound);
                OnFrameResult?.Invoke("Strike!");
            }
            else if (isSpare)
            {
                PlaySound(spareSound);
                OnFrameResult?.Invoke("Spare!");
            }
            else if (isGutterBall)
            {
                PlaySound(gutterBallSound);
                OnFrameResult?.Invoke("Gutter Ball!");
            }
            
            // Handle 10th frame special rules
            if (currentFrame == 9)
            {
                Handle10thFrame(isStrike, isSpare);
            }
            else
            {
                HandleNormalFrame(isStrike);
            }
            
            // Calculate score
            CalculateTotalScore();
            UpdateScoreDisplay();
            
            // Check for game over
            if (gameOver)
            {
                currentState = GameState.GameOver;
                OnGameOver?.Invoke();
            }
            else
            {
                // Reset for next throw
                currentState = GameState.ResettingPins;
                yield return new WaitForSeconds(pinResetDelay);
                
                bowlingBall.ResetBall();
                currentState = GameState.WaitingForThrow;
            }
        }
        
        private void HandleNormalFrame(bool isStrike)
        {
            currentThrow++;
            
            if (isStrike || !isFirstThrowInFrame)
            {
                // Frame complete
                currentFrame++;
                isFirstThrowInFrame = true;
                
                // Reset all pins for new frame
                bowlingLane.SetupPins();
            }
            else
            {
                // Second throw in frame
                isFirstThrowInFrame = false;
                
                // Remove knocked down pins
                bowlingLane.RemoveKnockedDownPins();
            }
        }
        
        private void Handle10thFrame(bool isStrike, bool isSpare)
        {
            currentThrow++;
            
            // In the 10th frame, player gets bonus throws for strikes and spares
            int throwsIn10th = currentThrow - GetThrowIndexForFrame(9);
            
            if (throwsIn10th == 1)
            {
                if (isStrike)
                {
                    // Bonus throw coming, reset pins
                    isFirstThrowInFrame = true;
                    bowlingLane.SetupPins();
                }
                else
                {
                    isFirstThrowInFrame = false;
                    bowlingLane.RemoveKnockedDownPins();
                }
            }
            else if (throwsIn10th == 2)
            {
                if (isStrike || isSpare)
                {
                    // Another bonus throw
                    isFirstThrowInFrame = true;
                    bowlingLane.SetupPins();
                }
                else if (throwScores[currentThrow - 2] == 10) // First throw was a strike
                {
                    if (isStrike)
                    {
                        bowlingLane.SetupPins();
                    }
                    else
                    {
                        bowlingLane.RemoveKnockedDownPins();
                        isFirstThrowInFrame = false;
                    }
                }
                else
                {
                    // No bonus throws
                    gameOver = true;
                }
            }
            else
            {
                // Third throw complete
                gameOver = true;
            }
        }
        
        private int GetThrowIndexForFrame(int frame)
        {
            // Returns the starting throw index for a given frame
            int index = 0;
            for (int f = 0; f < frame; f++)
            {
                if (throwScores[index] == 10 && f < 9) // Strike (not in 10th frame)
                {
                    index++;
                }
                else
                {
                    index += 2;
                }
            }
            return index;
        }
        
        private void CalculateTotalScore()
        {
            totalScore = 0;
            int throwIndex = 0;
            
            for (int frame = 0; frame < 10; frame++)
            {
                if (throwIndex >= currentThrow + 1) break;
                
                if (throwScores[throwIndex] == 10) // Strike
                {
                    frameScores[frame] = 10;
                    
                    // Add bonus (next two throws)
                    if (throwIndex + 1 < 21) frameScores[frame] += throwScores[throwIndex + 1];
                    if (throwIndex + 2 < 21) frameScores[frame] += throwScores[throwIndex + 2];
                    
                    throwIndex++;
                }
                else if (throwIndex + 1 < 21 && throwScores[throwIndex] + throwScores[throwIndex + 1] == 10) // Spare
                {
                    frameScores[frame] = 10;
                    
                    // Add bonus (next throw)
                    if (throwIndex + 2 < 21) frameScores[frame] += throwScores[throwIndex + 2];
                    
                    throwIndex += 2;
                }
                else // Normal
                {
                    frameScores[frame] = throwScores[throwIndex];
                    if (throwIndex + 1 < 21) frameScores[frame] += throwScores[throwIndex + 1];
                    
                    throwIndex += 2;
                }
                
                totalScore += frameScores[frame];
            }
        }
        
        private void UpdateScoreDisplay()
        {
            if (scoreDisplay != null)
            {
                scoreDisplay.UpdateScore(frameScores, totalScore, currentFrame);
            }
            
            OnScoreUpdated?.Invoke(currentFrame, totalScore);
        }
        
        private void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        /// <summary>
        /// Get the score for a specific frame.
        /// </summary>
        public int GetFrameScore(int frame)
        {
            if (frame >= 0 && frame < 10)
            {
                return frameScores[frame];
            }
            return 0;
        }
        
        /// <summary>
        /// Start a new game.
        /// </summary>
        public void NewGame()
        {
            InitializeGame();
        }
    }
}

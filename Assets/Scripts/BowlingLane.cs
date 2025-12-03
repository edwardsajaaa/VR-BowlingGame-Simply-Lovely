using UnityEngine;
using System.Collections.Generic;

namespace VRBowling.Scripts
{
    /// <summary>
    /// Manages the bowling lane, including pin setup and ball return.
    /// </summary>
    public class BowlingLane : MonoBehaviour
    {
        [Header("Lane References")]
        [SerializeField] private Transform pinAreaStart;
        [SerializeField] private Transform ballSpawnPoint;
        [SerializeField] private Transform ballReturnPoint;
        
        [Header("Pin Settings")]
        [SerializeField] private GameObject pinPrefab;
        [SerializeField] private float pinSpacing = 0.3f;
        [SerializeField] private float rowSpacing = 0.26f;
        
        [Header("Lane Dimensions")]
        [SerializeField] private float laneLength = 18.29f; // Standard bowling lane length in meters
        [SerializeField] private float laneWidth = 1.05f; // Standard bowling lane width in meters
        
        private List<BowlingPin> pins = new List<BowlingPin>();
        private BowlingBall currentBall;
        
        public List<BowlingPin> Pins => pins;
        public BowlingBall CurrentBall => currentBall;
        
        public delegate void LaneEventHandler();
        public event LaneEventHandler OnPinsSetup;
        public event LaneEventHandler OnPinsCleared;
        
        private void Start()
        {
            SetupPins();
        }
        
        /// <summary>
        /// Setup all 10 pins in standard bowling formation.
        /// Pin arrangement (looking from ball's perspective):
        ///       7  8  9  10
        ///         4  5  6
        ///           2  3
        ///             1
        /// </summary>
        public void SetupPins()
        {
            ClearPins();
            
            if (pinPrefab == null || pinAreaStart == null)
            {
                Debug.LogWarning("Pin prefab or pin area start not set!");
                return;
            }
            
            // Pin positions in standard bowling formation
            // Row 1 (closest to ball): 1 pin
            // Row 2: 2 pins
            // Row 3: 3 pins
            // Row 4 (farthest from ball): 4 pins
            
            int pinNumber = 1;
            
            for (int row = 0; row < 4; row++)
            {
                int pinsInRow = row + 1;
                float rowOffset = row * rowSpacing;
                float startX = -(pinsInRow - 1) * pinSpacing / 2f;
                
                for (int col = 0; col < pinsInRow; col++)
                {
                    Vector3 pinPosition = pinAreaStart.position + new Vector3(
                        startX + col * pinSpacing,
                        0,
                        rowOffset
                    );
                    
                    GameObject pinObj = Instantiate(pinPrefab, pinPosition, Quaternion.identity, transform);
                    pinObj.name = $"Pin_{pinNumber}";
                    
                    BowlingPin pin = pinObj.GetComponent<BowlingPin>();
                    if (pin == null)
                    {
                        pin = pinObj.AddComponent<BowlingPin>();
                    }
                    
                    pin.Initialize(pinNumber);
                    pin.SetStartPosition(pinPosition, Quaternion.identity);
                    pins.Add(pin);
                    
                    pinNumber++;
                }
            }
            
            OnPinsSetup?.Invoke();
        }
        
        /// <summary>
        /// Clear all pins from the lane.
        /// </summary>
        public void ClearPins()
        {
            foreach (var pin in pins)
            {
                if (pin != null)
                {
                    Destroy(pin.gameObject);
                }
            }
            pins.Clear();
            OnPinsCleared?.Invoke();
        }
        
        /// <summary>
        /// Reset only the standing pins (for second throw in a frame).
        /// </summary>
        public void ResetStandingPins()
        {
            foreach (var pin in pins)
            {
                if (pin != null && !pin.IsKnockedDown)
                {
                    pin.ResetPin();
                }
            }
        }
        
        /// <summary>
        /// Reset all pins to their starting positions.
        /// </summary>
        public void ResetAllPins()
        {
            foreach (var pin in pins)
            {
                if (pin != null)
                {
                    pin.ResetPin();
                }
            }
        }
        
        /// <summary>
        /// Get the count of pins that have been knocked down.
        /// </summary>
        public int GetKnockedDownPinCount()
        {
            int count = 0;
            foreach (var pin in pins)
            {
                if (pin != null && pin.IsKnockedDown)
                {
                    count++;
                }
            }
            return count;
        }
        
        /// <summary>
        /// Get the count of pins still standing.
        /// </summary>
        public int GetStandingPinCount()
        {
            return 10 - GetKnockedDownPinCount();
        }
        
        /// <summary>
        /// Check if all pins have been knocked down (strike or spare).
        /// </summary>
        public bool AllPinsDown()
        {
            return GetKnockedDownPinCount() == 10;
        }
        
        /// <summary>
        /// Register a bowling ball with this lane.
        /// </summary>
        public void RegisterBall(BowlingBall ball)
        {
            currentBall = ball;
            if (ballSpawnPoint != null)
            {
                ball.SetStartPosition(ballSpawnPoint.position);
            }
        }
        
        /// <summary>
        /// Get the ball spawn position.
        /// </summary>
        public Vector3 GetBallSpawnPosition()
        {
            return ballSpawnPoint != null ? ballSpawnPoint.position : Vector3.zero;
        }
        
        /// <summary>
        /// Get the ball return position.
        /// </summary>
        public Vector3 GetBallReturnPosition()
        {
            return ballReturnPoint != null ? ballReturnPoint.position : Vector3.zero;
        }
        
        /// <summary>
        /// Remove knocked down pins from the lane (visual sweep).
        /// </summary>
        public void RemoveKnockedDownPins()
        {
            List<BowlingPin> pinsToRemove = new List<BowlingPin>();
            
            foreach (var pin in pins)
            {
                if (pin != null && pin.IsKnockedDown)
                {
                    pinsToRemove.Add(pin);
                }
            }
            
            foreach (var pin in pinsToRemove)
            {
                pins.Remove(pin);
                Destroy(pin.gameObject);
            }
        }
    }
}

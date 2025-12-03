using UnityEngine;

namespace VRBowling.Scripts
{
    /// <summary>
    /// Controls bowling pin physics and knockdown detection.
    /// Attach to each bowling pin GameObject.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class BowlingPin : MonoBehaviour
    {
        [Header("Pin Settings")]
        [SerializeField] private float knockdownAngleThreshold = 45f;
        [SerializeField] private float pinWeight = 1.5f; // Standard bowling pin weight in kg
        
        [Header("Audio")]
        [SerializeField] private AudioClip pinHitSound;
        [SerializeField] private AudioClip pinFallSound;
        
        private Rigidbody rb;
        private AudioSource audioSource;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private bool isKnockedDown;
        private int pinNumber;
        
        public bool IsKnockedDown => isKnockedDown;
        public int PinNumber => pinNumber;
        
        public delegate void PinEventHandler(BowlingPin pin);
        public event PinEventHandler OnPinKnockedDown;
        public event PinEventHandler OnPinReset;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
            
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            // Store initial position for reset
            startPosition = transform.position;
            startRotation = transform.rotation;
            
            // Configure rigidbody for bowling pin physics
            rb.mass = pinWeight;
            rb.drag = 0.5f;
            rb.angularDrag = 0.5f;
            
            // Set tag for collision detection
            gameObject.tag = "BowlingPin";
        }
        
        private void Update()
        {
            // Check if pin has been knocked down
            if (!isKnockedDown)
            {
                float angle = Vector3.Angle(Vector3.up, transform.up);
                if (angle > knockdownAngleThreshold)
                {
                    SetKnockedDown(true);
                }
            }
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            // Play hit sound when colliding with ball or other pins
            if (collision.gameObject.CompareTag("BowlingBall") || 
                collision.gameObject.CompareTag("BowlingPin"))
            {
                PlayHitSound(collision.relativeVelocity.magnitude);
            }
        }
        
        private void PlayHitSound(float impactForce)
        {
            if (pinHitSound != null && audioSource != null)
            {
                // Adjust volume based on impact force
                float volume = Mathf.Clamp01(impactForce / 10f);
                audioSource.PlayOneShot(pinHitSound, volume);
            }
        }
        
        private void PlayFallSound()
        {
            if (pinFallSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pinFallSound);
            }
        }
        
        /// <summary>
        /// Set the pin's knocked down state.
        /// </summary>
        private void SetKnockedDown(bool knocked)
        {
            if (isKnockedDown != knocked)
            {
                isKnockedDown = knocked;
                if (isKnockedDown)
                {
                    PlayFallSound();
                    OnPinKnockedDown?.Invoke(this);
                }
            }
        }
        
        /// <summary>
        /// Initialize the pin with a number (1-10).
        /// </summary>
        public void Initialize(int number)
        {
            pinNumber = number;
        }
        
        /// <summary>
        /// Reset the pin to its starting position.
        /// </summary>
        public void ResetPin()
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = startPosition;
            transform.rotation = startRotation;
            isKnockedDown = false;
            rb.isKinematic = false;
            OnPinReset?.Invoke(this);
        }
        
        /// <summary>
        /// Set a new starting position for the pin.
        /// </summary>
        public void SetStartPosition(Vector3 position, Quaternion rotation)
        {
            startPosition = position;
            startRotation = rotation;
        }
        
        /// <summary>
        /// Check if the pin is still standing (not knocked down).
        /// </summary>
        public bool IsStanding()
        {
            return !isKnockedDown;
        }
        
        /// <summary>
        /// Temporarily freeze the pin (used during reset animation).
        /// </summary>
        public void Freeze()
        {
            rb.isKinematic = true;
        }
        
        /// <summary>
        /// Unfreeze the pin.
        /// </summary>
        public void Unfreeze()
        {
            rb.isKinematic = false;
        }
    }
}

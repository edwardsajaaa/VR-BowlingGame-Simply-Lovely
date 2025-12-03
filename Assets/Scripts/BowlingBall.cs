using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRBowling.Scripts
{
    /// <summary>
    /// Controls the bowling ball physics and VR interaction.
    /// Attach to a bowling ball GameObject with Rigidbody and XR Grab Interactable.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(XRGrabInteractable))]
    public class BowlingBall : MonoBehaviour
    {
        [Header("Ball Settings")]
        [SerializeField] private float ballWeight = 6f; // Ball weight in kg (typical bowling ball)
        [SerializeField] private float maxThrowForce = 15f;
        [SerializeField] private float gutterSpeed = 0.5f;
        
        [Header("Audio")]
        [SerializeField] private AudioClip rollSound;
        [SerializeField] private AudioClip hitPinSound;
        
        private Rigidbody rb;
        private XRGrabInteractable grabInteractable;
        private AudioSource audioSource;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private bool isThrown;
        private bool isInGutter;
        
        public bool IsThrown => isThrown;
        public bool IsInGutter => isInGutter;
        
        public delegate void BallEventHandler(BowlingBall ball);
        public event BallEventHandler OnBallThrown;
        public event BallEventHandler OnBallStopped;
        public event BallEventHandler OnBallReset;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            grabInteractable = GetComponent<XRGrabInteractable>();
            audioSource = GetComponent<AudioSource>();
            
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            // Store initial position for reset
            startPosition = transform.position;
            startRotation = transform.rotation;
            
            // Configure rigidbody for bowling ball physics
            rb.mass = ballWeight;
            rb.drag = 0.1f;
            rb.angularDrag = 0.5f;
        }
        
        private void OnEnable()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectExited.AddListener(OnRelease);
                grabInteractable.selectEntered.AddListener(OnGrab);
            }
        }
        
        private void OnDisable()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectExited.RemoveListener(OnRelease);
                grabInteractable.selectEntered.RemoveListener(OnGrab);
            }
        }
        
        private void OnGrab(SelectEnterEventArgs args)
        {
            // Ball has been grabbed
            isThrown = false;
            isInGutter = false;
            StopRollSound();
        }
        
        private void OnRelease(SelectExitEventArgs args)
        {
            isThrown = true;
            OnBallThrown?.Invoke(this);
            
            // Clamp the throw velocity if too strong
            if (rb.linearVelocity.magnitude > maxThrowForce)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxThrowForce;
            }
        }
        
        private void Update()
        {
            // Check if ball has stopped moving
            if (isThrown && rb.linearVelocity.magnitude < 0.1f && rb.angularVelocity.magnitude < 0.1f)
            {
                isThrown = false;
                OnBallStopped?.Invoke(this);
                StopRollSound();
            }
            
            // Play rolling sound when ball is moving
            if (isThrown && rb.linearVelocity.magnitude > 0.5f)
            {
                PlayRollSound();
            }
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            // Check if ball hit a pin
            if (collision.gameObject.CompareTag("BowlingPin"))
            {
                PlayHitSound();
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // Check if ball entered gutter
            if (other.CompareTag("Gutter"))
            {
                isInGutter = true;
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, gutterSpeed);
            }
        }
        
        private void PlayRollSound()
        {
            if (rollSound != null && audioSource != null && !audioSource.isPlaying)
            {
                audioSource.clip = rollSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        
        private void StopRollSound()
        {
            if (audioSource != null && audioSource.isPlaying && audioSource.clip == rollSound)
            {
                audioSource.Stop();
            }
        }
        
        private void PlayHitSound()
        {
            if (hitPinSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitPinSound);
            }
        }
        
        /// <summary>
        /// Reset the ball to its starting position.
        /// </summary>
        public void ResetBall()
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = startPosition;
            transform.rotation = startRotation;
            isThrown = false;
            isInGutter = false;
            OnBallReset?.Invoke(this);
        }
        
        /// <summary>
        /// Set a new starting position for the ball.
        /// </summary>
        public void SetStartPosition(Vector3 position)
        {
            startPosition = position;
        }
    }
}

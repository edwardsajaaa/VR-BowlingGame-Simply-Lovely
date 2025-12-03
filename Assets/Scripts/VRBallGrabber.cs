using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRBowling.Scripts
{
    /// <summary>
    /// Enhanced VR ball grabber that provides haptic feedback and throw assistance.
    /// Attach to the bowling ball along with XRGrabInteractable.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class VRBallGrabber : MonoBehaviour
    {
        [Header("Grab Settings")]
        [SerializeField] private float grabDistance = 0.5f;
        [SerializeField] private bool useGravityWhenHeld = false;
        [SerializeField] private bool trackVelocity = true;
        
        [Header("Haptic Feedback")]
        [SerializeField] private float grabHapticIntensity = 0.3f;
        [SerializeField] private float grabHapticDuration = 0.1f;
        [SerializeField] private float throwHapticIntensity = 0.5f;
        [SerializeField] private float throwHapticDuration = 0.2f;
        
        [Header("Throw Assistance")]
        [SerializeField] private bool enableThrowAssistance = true;
        [SerializeField] private float minThrowSpeed = 2f;
        [SerializeField] private float maxThrowSpeed = 15f;
        [SerializeField] private float throwMultiplier = 1.2f;
        
        [Header("Visual Feedback")]
        [SerializeField] private Material highlightMaterial;
        [SerializeField] private Color grabHighlightColor = Color.cyan;
        
        private XRGrabInteractable grabInteractable;
        private Rigidbody rb;
        private Renderer ballRenderer;
        private Material originalMaterial;
        private IXRSelectInteractor currentInteractor;
        
        // Velocity tracking for throw assistance
        private Vector3[] velocityHistory = new Vector3[10];
        private int velocityIndex = 0;
        private bool wasGrabbed = false;
        
        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            rb = GetComponent<Rigidbody>();
            ballRenderer = GetComponent<Renderer>();
            
            if (ballRenderer != null)
            {
                originalMaterial = ballRenderer.material;
            }
            
            ConfigureGrabInteractable();
        }
        
        private void ConfigureGrabInteractable()
        {
            if (grabInteractable != null)
            {
                // Configure for bowling ball behavior
                grabInteractable.throwOnDetach = true;
                grabInteractable.trackPosition = true;
                grabInteractable.trackRotation = true;
                grabInteractable.smoothPosition = true;
                grabInteractable.smoothRotation = true;
                grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
            }
        }
        
        private void OnEnable()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.AddListener(OnGrab);
                grabInteractable.selectExited.AddListener(OnRelease);
                grabInteractable.hoverEntered.AddListener(OnHoverEnter);
                grabInteractable.hoverExited.AddListener(OnHoverExit);
            }
        }
        
        private void OnDisable()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.RemoveListener(OnGrab);
                grabInteractable.selectExited.RemoveListener(OnRelease);
                grabInteractable.hoverEntered.RemoveListener(OnHoverEnter);
                grabInteractable.hoverExited.RemoveListener(OnHoverExit);
            }
        }
        
        private void Update()
        {
            if (wasGrabbed && trackVelocity)
            {
                TrackVelocity();
            }
        }
        
        private void TrackVelocity()
        {
            if (rb != null)
            {
                velocityHistory[velocityIndex] = rb.linearVelocity;
                velocityIndex = (velocityIndex + 1) % velocityHistory.Length;
            }
        }
        
        private void OnGrab(SelectEnterEventArgs args)
        {
            currentInteractor = args.interactorObject;
            wasGrabbed = true;
            
            // Disable gravity while holding
            if (!useGravityWhenHeld)
            {
                rb.useGravity = false;
            }
            
            // Send haptic feedback
            SendHapticFeedback(grabHapticIntensity, grabHapticDuration);
            
            // Reset velocity history
            for (int i = 0; i < velocityHistory.Length; i++)
            {
                velocityHistory[i] = Vector3.zero;
            }
        }
        
        private void OnRelease(SelectExitEventArgs args)
        {
            // Re-enable gravity
            rb.useGravity = true;
            
            // Apply throw assistance
            if (enableThrowAssistance)
            {
                ApplyThrowAssistance();
            }
            
            // Send haptic feedback
            SendHapticFeedback(throwHapticIntensity, throwHapticDuration);
            
            wasGrabbed = false;
            currentInteractor = null;
        }
        
        private void OnHoverEnter(HoverEnterEventArgs args)
        {
            // Highlight ball when hovering
            if (highlightMaterial != null && ballRenderer != null)
            {
                ballRenderer.material = highlightMaterial;
            }
            else if (ballRenderer != null)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                ballRenderer.GetPropertyBlock(block);
                block.SetColor("_EmissionColor", grabHighlightColor * 0.3f);
                ballRenderer.SetPropertyBlock(block);
            }
        }
        
        private void OnHoverExit(HoverExitEventArgs args)
        {
            // Remove highlight
            if (originalMaterial != null && ballRenderer != null)
            {
                ballRenderer.material = originalMaterial;
            }
            else if (ballRenderer != null)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                ballRenderer.GetPropertyBlock(block);
                block.SetColor("_EmissionColor", Color.black);
                ballRenderer.SetPropertyBlock(block);
            }
        }
        
        private void ApplyThrowAssistance()
        {
            // Calculate average velocity from history
            Vector3 avgVelocity = Vector3.zero;
            int validSamples = 0;
            
            for (int i = 0; i < velocityHistory.Length; i++)
            {
                if (velocityHistory[i].magnitude > 0.1f)
                {
                    avgVelocity += velocityHistory[i];
                    validSamples++;
                }
            }
            
            if (validSamples > 0)
            {
                avgVelocity /= validSamples;
            }
            
            // Apply throw multiplier if within speed range
            float speed = avgVelocity.magnitude;
            if (speed >= minThrowSpeed && speed <= maxThrowSpeed)
            {
                Vector3 assistedVelocity = avgVelocity * throwMultiplier;
                
                // Ensure forward direction is maintained (toward pins)
                if (assistedVelocity.z > 0)
                {
                    rb.linearVelocity = assistedVelocity;
                }
            }
        }
        
        private void SendHapticFeedback(float intensity, float duration)
        {
            if (currentInteractor is XRBaseControllerInteractor controllerInteractor)
            {
                var controller = controllerInteractor.xrController;
                if (controller != null)
                {
                    controller.SendHapticImpulse(intensity, duration);
                }
            }
        }
        
        /// <summary>
        /// Set the throw assistance multiplier.
        /// </summary>
        public void SetThrowMultiplier(float multiplier)
        {
            throwMultiplier = Mathf.Clamp(multiplier, 0.5f, 2f);
        }
        
        /// <summary>
        /// Enable or disable throw assistance.
        /// </summary>
        public void SetThrowAssistance(bool enabled)
        {
            enableThrowAssistance = enabled;
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VRBowling.Scripts
{
    /// <summary>
    /// Handles the pin reset mechanism with visual sweep animation.
    /// </summary>
    public class PinResetter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BowlingLane bowlingLane;
        [SerializeField] private Transform sweepBar;
        [SerializeField] private Transform pinSetterArm;
        
        [Header("Animation Settings")]
        [SerializeField] private float sweepDuration = 1.5f;
        [SerializeField] private float setterLowerDuration = 1f;
        [SerializeField] private float setterRaiseDuration = 1f;
        
        [Header("Positions")]
        [SerializeField] private Vector3 sweepStartPosition = new Vector3(0, 2f, -1f);
        [SerializeField] private Vector3 sweepEndPosition = new Vector3(0, 2f, 2f);
        [SerializeField] private Vector3 setterUpPosition = new Vector3(0, 3f, 0);
        [SerializeField] private Vector3 setterDownPosition = new Vector3(0, 0.5f, 0);
        
        [Header("Audio")]
        [SerializeField] private AudioClip sweepSound;
        [SerializeField] private AudioClip setterSound;
        
        private AudioSource audioSource;
        private bool isResetting;
        
        public bool IsResetting => isResetting;
        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        /// <summary>
        /// Perform a full pin reset (sweep and set new pins).
        /// </summary>
        public IEnumerator FullReset()
        {
            isResetting = true;
            
            // Sweep pins away
            yield return StartCoroutine(SweepAnimation());
            
            // Clear all pins
            if (bowlingLane != null)
            {
                bowlingLane.ClearPins();
            }
            
            // Lower pin setter with new pins
            yield return StartCoroutine(SetterLowerAnimation());
            
            // Set up new pins
            if (bowlingLane != null)
            {
                bowlingLane.SetupPins();
            }
            
            // Raise pin setter
            yield return StartCoroutine(SetterRaiseAnimation());
            
            isResetting = false;
        }
        
        /// <summary>
        /// Perform a partial reset (only clear fallen pins).
        /// </summary>
        public IEnumerator PartialReset()
        {
            isResetting = true;
            
            // Sweep knocked down pins
            yield return StartCoroutine(SweepAnimation());
            
            // Remove knocked down pins
            if (bowlingLane != null)
            {
                bowlingLane.RemoveKnockedDownPins();
            }
            
            isResetting = false;
        }
        
        private IEnumerator SweepAnimation()
        {
            if (sweepBar == null) yield break;
            
            PlaySound(sweepSound);
            
            float elapsed = 0f;
            sweepBar.gameObject.SetActive(true);
            sweepBar.localPosition = sweepStartPosition;
            
            while (elapsed < sweepDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / sweepDuration;
                sweepBar.localPosition = Vector3.Lerp(sweepStartPosition, sweepEndPosition, t);
                yield return null;
            }
            
            sweepBar.localPosition = sweepStartPosition;
            sweepBar.gameObject.SetActive(false);
        }
        
        private IEnumerator SetterLowerAnimation()
        {
            if (pinSetterArm == null) yield break;
            
            PlaySound(setterSound);
            
            float elapsed = 0f;
            pinSetterArm.gameObject.SetActive(true);
            pinSetterArm.localPosition = setterUpPosition;
            
            while (elapsed < setterLowerDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / setterLowerDuration;
                pinSetterArm.localPosition = Vector3.Lerp(setterUpPosition, setterDownPosition, t);
                yield return null;
            }
            
            pinSetterArm.localPosition = setterDownPosition;
        }
        
        private IEnumerator SetterRaiseAnimation()
        {
            if (pinSetterArm == null) yield break;
            
            float elapsed = 0f;
            
            while (elapsed < setterRaiseDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / setterRaiseDuration;
                pinSetterArm.localPosition = Vector3.Lerp(setterDownPosition, setterUpPosition, t);
                yield return null;
            }
            
            pinSetterArm.localPosition = setterUpPosition;
            pinSetterArm.gameObject.SetActive(false);
        }
        
        private void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        /// <summary>
        /// Start a full reset coroutine externally.
        /// </summary>
        public void TriggerFullReset()
        {
            if (!isResetting)
            {
                StartCoroutine(FullReset());
            }
        }
        
        /// <summary>
        /// Start a partial reset coroutine externally.
        /// </summary>
        public void TriggerPartialReset()
        {
            if (!isResetting)
            {
                StartCoroutine(PartialReset());
            }
        }
    }
}

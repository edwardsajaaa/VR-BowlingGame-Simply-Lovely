using UnityEngine;

namespace VRBowling.Scripts
{
    /// <summary>
    /// Simple audio manager for bowling game sounds.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [Header("Music")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private float musicVolume = 0.5f;
        
        [Header("Sound Effects")]
        [SerializeField] private AudioClip[] pinHitSounds;
        [SerializeField] private AudioClip ballRollSound;
        [SerializeField] private AudioClip strikeSound;
        [SerializeField] private AudioClip spareSound;
        [SerializeField] private AudioClip gutterSound;
        [SerializeField] private AudioClip crowdCheerSound;
        
        [Header("Settings")]
        [SerializeField] private float sfxVolume = 1f;
        
        private AudioSource musicSource;
        private AudioSource sfxSource;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Create audio sources
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.volume = sfxVolume;
        }
        
        private void Start()
        {
            PlayBackgroundMusic();
        }
        
        public void PlayBackgroundMusic()
        {
            if (backgroundMusic != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
            }
        }
        
        public void StopBackgroundMusic()
        {
            musicSource.Stop();
        }
        
        public void PlayPinHit()
        {
            if (pinHitSounds != null && pinHitSounds.Length > 0)
            {
                var randomClip = pinHitSounds[Random.Range(0, pinHitSounds.Length)];
                sfxSource.PlayOneShot(randomClip, sfxVolume);
            }
        }
        
        public void PlayBallRoll()
        {
            if (ballRollSound != null)
            {
                sfxSource.clip = ballRollSound;
                sfxSource.loop = true;
                sfxSource.Play();
            }
        }
        
        public void StopBallRoll()
        {
            if (sfxSource.clip == ballRollSound)
            {
                sfxSource.loop = false;
                sfxSource.Stop();
            }
        }
        
        public void PlayStrike()
        {
            if (strikeSound != null)
            {
                sfxSource.PlayOneShot(strikeSound, sfxVolume);
            }
            PlayCrowdCheer();
        }
        
        public void PlaySpare()
        {
            if (spareSound != null)
            {
                sfxSource.PlayOneShot(spareSound, sfxVolume);
            }
        }
        
        public void PlayGutter()
        {
            if (gutterSound != null)
            {
                sfxSource.PlayOneShot(gutterSound, sfxVolume);
            }
        }
        
        public void PlayCrowdCheer()
        {
            if (crowdCheerSound != null)
            {
                sfxSource.PlayOneShot(crowdCheerSound, sfxVolume * 0.7f);
            }
        }
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            musicSource.volume = musicVolume;
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            sfxSource.volume = sfxVolume;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Spider Matron Sounds")]
    public AudioClip SpiderScreech;
    // public AudioClip SpiderDeath;
    public AudioClip SpiderFootstep;
    [Header("Hatchling Sounds")]
    public AudioClip HatchlingFootstep;
    public AudioClip HatchlingExplosiveDeath;
    public AudioClip HatchEgg;
    [Header("Player Sounds")]
    public AudioClip CrouchWoosh;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    public float FootStepCrouchAudioVolume = 0.2f;

    [Header("background tracks and other effects")]
    public AudioClip TerrorAmbience; // Use near spiders
    public AudioClip NightAmbience; //Use away from spiders
    public AudioClip BeingDetected; // Use when beinng detected
    public AudioClip BoatOnWater;
    public AudioClip WindyStartMenu;
    public AudioClip DropEggAtShip;
    private AudioSource backgroundSource;
    private Coroutine fadeCoroutine;
    public bool playedOnce = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else
        {
            Destroy(gameObject);
            return;
        }
        backgroundSource = gameObject.GetComponent<AudioSource>();
        backgroundSource.loop = true;
        
    }
    // Start is called before the first frame update
    void Start()
    {
        // PlayBackgroundMusic(NightAmbience, 0.5f);
        PlayBackgroundMusic(WindyStartMenu, 0.7f);
    }

    public void PlayBackgroundMusic(AudioClip clip, float volume)
    {
        // backgroundSource.clip = clip;
        // backgroundSource.Play();

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeInMusic(clip, volume));
    }

    public void TurnOffAudioSourceLoop()
    {
        backgroundSource.loop = false;
    }
    public void TurnOnAudioSourceLoop()
    {
        backgroundSource.loop = true;
    }

    private IEnumerator FadeInMusic(AudioClip clip, float volume)
    {
        // Fade in new music and fade out old music
        float fadeDuration = 1f;
        float startVolume = backgroundSource.volume;

        //fadeout
        while (backgroundSource.volume > 0)
        {
            backgroundSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;

        }
        //switch clip once faded out
        backgroundSource.clip = clip;
        backgroundSource.volume = 0;//start at 0
        backgroundSource.loop = true;
        backgroundSource.Play();

        //fade in new music
        while (backgroundSource.volume < volume)
        {
            backgroundSource.volume += volume * Time.deltaTime / fadeDuration;
            yield return null;
        }
        backgroundSource.volume = volume;
        fadeCoroutine = null;
    }

    public void PlayImmediate(AudioClip newClip, float volume)
    {
        if(fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        backgroundSource.volume = volume;
        backgroundSource.clip = newClip;
        backgroundSource.loop = false;
        backgroundSource.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySoundAtPosition(AudioClip clip, Vector3 position)
    {
        //change pitch randomly between 0.9 and 1.1
        float pitch = Random.Range(0.9f, 1.1f);
        AudioSource.PlayClipAtPoint(clip, position, pitch);
        // edit volume


        
    }

    
    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float Volumelow, float Volumehigh)
    {
        float pitch = Random.Range(Volumelow, Volumehigh);
        AudioSource.PlayClipAtPoint(clip, position, pitch);
    }

    public void OnFootstep(Vector3 Position, float Volume)
        {
            
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(Position), Volume);
            }
            
        }

    public void StopAllAudio()
    {
        backgroundSource.Stop(); // Stops the background music
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in allAudioSources)
        {
            audioSource.Stop(); // Stops all active AudioSources
        }
    }

}

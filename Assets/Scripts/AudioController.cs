using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;

        source = GetComponent<AudioSource>();

        audioClips[geyserSFXname] = geyserSFX;
        audioClips[stepSFXname] = stepSFX;
        audioClips[hitSFXname] = hitSFX;
    }

    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    
    public string geyserSFXname;
    public AudioClip geyserSFX;
    
    public string stepSFXname;
    public AudioClip stepSFX;
    
    public string hitSFXname;
    public AudioClip hitSFX;

    private AudioSource source;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddAudioClip(string clipName, AudioClip clip)
    {
        audioClips[clipName] = clip;
    }

    public void PlayAudio(string clipName, float volume = 1f)
    {
        source.PlayOneShot(audioClips[clipName], volume);
    }

    public AudioSource GetSource()
    {
        return source;
    }
}

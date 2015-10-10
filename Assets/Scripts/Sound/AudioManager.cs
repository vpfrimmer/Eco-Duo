using UnityEngine;
using SwissArmyKnife;

/// <summary>
/// Allows basic sound control.
/// Never link the Play function to a button, but use the AudioCaller Component.
/// </summary>
public class AudioManager : SingletonPersistent<AudioManager>
{

    public float noiseVolume = 1;
    public float musicVolume = 1;

    public AudioSource noise;
    public AudioSource music;
    bool isNoiseOn = true;

    public void Play(AudioClip clip)
    {
        if (isNoiseOn)
        {
            noise.PlayOneShot(clip);
        }
    }

    void Start()
    {
        noise.volume = noiseVolume;
        music.volume = musicVolume;

        music.Play();
    }

    public void SetMusicState(bool value)
    {
        if (value)
        {
            music.UnPause();
        }
        else
        {
            music.Pause();
        }
    }

    public void SetSoundState(bool value)
    {
        isNoiseOn = value;
        if (value)
        {
            noise.volume = noiseVolume;
        }
        else
        {
            noise.volume = 0;
        }
    }
}
using UnityEngine;
using SwissArmyKnife;
using System.Collections;

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

    public void Play(AudioClip clip, float volume = 1f, float time = 0f)
    {
        if (isNoiseOn)
        {
            if (time > 0f)
            {
                StartCoroutine(PlayCoroutine(clip, volume, time));
            }
            else
            {
                noise.PlayOneShot(clip, volume);
            }
        }
    }

    IEnumerator PlayCoroutine(AudioClip clip, float volume, float time)
    {
        yield return new WaitForSeconds(time);

        noise.PlayOneShot(clip, volume);
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
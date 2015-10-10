using UnityEngine;
using System.Collections;

/// <summary>
/// Calls the Play method from the AudioManager Singleton
/// Allows the game to have sound when starting from any scene,
/// as hard links would fail when using another scene's AudioManager
/// </summary>
public class AudioCaller : MonoBehaviour
{
    public void Play(AudioClip clip)
    {
        AudioManager.Instance.Play(clip);
    }
}
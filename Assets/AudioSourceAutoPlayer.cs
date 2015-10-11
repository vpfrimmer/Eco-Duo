using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[System.Serializable]
public class AudioSourceAutoPlayer : MonoBehaviour
{
    private bool isActive = true;

    public bool test;

    public bool active
    {
        set
        {
            if (value)
            {
                AudioSource source = GetComponent<AudioSource>();

                source.Play();
            }

            isActive = value;
        }
        get
        {
            return (isActive);
        }
    }
}
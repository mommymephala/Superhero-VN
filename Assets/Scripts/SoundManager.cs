using System;
using UnityEngine;

[Serializable]
public class SoundClip
{
    public AudioClip clip;
    public float volume = 1.0f;
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private SoundClip[] soundClips;

    public void PlaySound(string soundName, bool isUISound = false, bool loop = false)
    {
        var source = isUISound ? uiAudioSource : backgroundAudioSource;
        var soundClip = Array.Find(soundClips, s => s.clip.name.Equals(soundName, StringComparison.OrdinalIgnoreCase));

        if (soundClip?.clip != null)
        {
            if (!isUISound && source.isPlaying)
            {
                source.Stop();
            }
            source.clip = soundClip.clip;
            source.volume = soundClip.volume;
            source.loop = loop;
            source.Play();
        }
        else
        {
            Debug.LogError("Sound not found: " + soundName);
        }
    }

    public void PlayButtonClickSound()
    {
        uiAudioSource?.PlayOneShot(buttonClickSound);
    }
}
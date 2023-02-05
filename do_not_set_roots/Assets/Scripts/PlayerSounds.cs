using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _clips;
    private AudioSource _audioSource;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    public void RunningSound()
    {
        AudioClip c = _clips[0];
        _audioSource.PlayOneShot(c);
    }

    public void RootsBreakSound()
    {
        AudioClip c = _clips[1];
        _audioSource.PlayOneShot(c);
    }
    
    public void TreeShakeSound()
    {
        AudioClip c = _clips[2];
        _audioSource.PlayOneShot(c);
    }
    
    public void StopSoundEffects()
    {
        _audioSource.Stop();
    }
    
}

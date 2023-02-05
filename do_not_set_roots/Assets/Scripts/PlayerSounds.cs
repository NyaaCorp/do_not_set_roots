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
        _audioSource.Stop();
        AudioClip c = _clips[0];
        _audioSource.PlayOneShot(c);
    }   
    
}

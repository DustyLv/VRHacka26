using System;
using UnityEngine;

public class PlayBloodSplatter : MonoBehaviour
{
    public ParticleSystem bloodSplatter;
    public AudioSource bloodSplatterAudioSource;
    
    public static PlayBloodSplatter instance;

    private void Awake()
    {
        instance = this;
    }


    
    public void Play()
    {
        bloodSplatter.Play();
        bloodSplatterAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        bloodSplatterAudioSource.Play();
    }
}

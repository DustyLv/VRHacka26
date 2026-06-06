using System;
using UnityEngine;

public class PlayBloodSplatter : MonoBehaviour
{
    public ParticleSystem bloodSplatter;
    
    public static PlayBloodSplatter instance;

    private void Awake()
    {
        instance = this;
    }


    
    public void Play()
    {
        bloodSplatter.Play();
    }
}

using UnityEngine;

public class GlobalAudioPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    
    public AudioClip[] audioClips_NPCHit;
    
    public static GlobalAudioPlayer instance;
    
    private void Awake()
    {
        instance = this;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAudio(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    
    public void PlayAudioNPCHit()
    {
        PlayAudio(audioClips_NPCHit[Random.Range(0, audioClips_NPCHit.Length)]);
    }
}

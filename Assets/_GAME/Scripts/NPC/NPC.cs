using System;
using Assets._GAME.Scripts.Hospital;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPC : MonoBehaviour
{
    // public Controller hospital;
    // public GameObject splatterParticles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Instantiate(splatterParticles, transform.position + (Vector3.up * 1.5f), Quaternion.identity);
        if (other.CompareTag("Player"))
        {
            PlayBloodSplatter.instance.Play();
            GlobalAudioPlayer.instance.PlayAudioNPCHit();
            int organType = Random.Range(0, 4);
            Controller.instance.Collect((OrganType)organType);
        }

        NPCSpawner.instance.SpawnNPCRandom();
        Destroy(gameObject);

    }
}

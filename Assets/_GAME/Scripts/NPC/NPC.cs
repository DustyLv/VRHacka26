using System;
using Unity.Mathematics;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject splatterParticles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }




    private void OnTriggerEnter(Collider other)
    {
        // Instantiate(splatterParticles, transform.position + (Vector3.up * 1.5f), Quaternion.identity);
        PlayBloodSplatter.instance.Play();
        Destroy(gameObject);

    }
}

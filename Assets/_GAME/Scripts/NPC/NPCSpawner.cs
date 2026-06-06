using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public Transform[] spawnpoints;
    public GameObject[] npcs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnNPC();
        // SpawnNPC();
    }
    

    public void SpawnNPC()
    {
        foreach (var spawnpoint in spawnpoints)
        {
            GameObject npc = Instantiate(npcs[Random.Range(0, npcs.Length)], spawnpoint.position, spawnpoint.rotation);   
        }
    }
}
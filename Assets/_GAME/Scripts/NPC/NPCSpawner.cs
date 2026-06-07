using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public Transform[] spawnpoints;
    public GameObject[] npcs;

    public static NPCSpawner instance;
    
    private void Awake()
    {
        instance = this;
    }
    
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
    
    public void SpawnNPCRandom()
    {
            Transform spawnpoint = spawnpoints[Random.Range(0, spawnpoints.Length)];
            GameObject npc = Instantiate(npcs[Random.Range(0, npcs.Length)], spawnpoint.position, spawnpoint.rotation);   
        
    }
}
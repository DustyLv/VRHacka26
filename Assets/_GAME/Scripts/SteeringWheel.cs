using UnityEngine;

public class SteeringWheel : MonoBehaviour
{
    public G27QuestController g27QuestController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 100 * Time.deltaTime);
    }
}

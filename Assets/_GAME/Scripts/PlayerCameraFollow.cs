using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    public Transform target;
    public float distance = 0;
    public float sideOffset = 0f;
    // public float height = 0f;
    public float heightOffset = 0f;
    public float rotationDamping = 3f;
    public float positionDamping = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null) return;
        
        transform.rotation = target.rotation;
        transform.position = target.position;
        return;
        
        // Copy the target's position into the object's position
        Vector3 targetPosition = target.position;
        targetPosition.y += heightOffset;
        targetPosition.x += sideOffset;
        
        // Adjust the Z position to be at the desired distance
        targetPosition -= transform.forward * distance;
        transform.position = targetPosition;

        // Adjust the height
        // transform.position = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
        
        // Copy the target's rotation into the object's rotation
        
    }
}

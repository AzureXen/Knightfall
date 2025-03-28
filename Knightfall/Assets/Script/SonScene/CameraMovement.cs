using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;  // Reference to the player
    public float height = 10f; // Camera height above the player
    public float smoothSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Keep the camera directly above the player, maintaining the height
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}   

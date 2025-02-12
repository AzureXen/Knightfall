using Unity.Cinemachine;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    CinemachineCamera cam;
    Transform target;

    void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        FindPlayer();
    }

    void Update()
    {
        if (target == null)
        {
            FindPlayer();
        }
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            cam.Follow = target;
            cam.LookAt = target;
        }
    }
}

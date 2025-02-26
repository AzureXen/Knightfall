using System;
using UnityEngine;

public class PlayerSwordVisual : MonoBehaviour
{
    private Camera mainCam;

    private Vector3 mousePos;

    private bool lastFlipState;
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 playerToMouse = mousePos - transform.position;

        float rotateZ = Mathf.Atan2(playerToMouse.y, playerToMouse.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotateZ);
    }
    
}

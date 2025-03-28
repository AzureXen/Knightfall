using System;
using UnityEngine;


// In RoninAnimator, there is only implementation for ronin to look at player.
// Currently, Ronin Animations are managed in RoninAction
public class RoninAnimator : MonoBehaviour
{
    private GameObject player;
    private SpriteRenderer sr;
    // canChangeDirection: Ronin's sprite will flip based on position of player, making him look at player.
    public Boolean canChangeDirection { get; set; } = false;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(canChangeDirection)
        {
            UpdateDirection();
        }
    }
    private void UpdateDirection()
    {
        if(player!=null)
        {
            Vector3 playerPos = player.transform.position;
            Vector3 playerDir = playerPos - transform.position;
            if (playerDir.x > 0)
            {
                sr.flipX = false;
            }
            else sr.flipX = true;
        }
    }
}

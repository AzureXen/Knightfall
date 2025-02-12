using System;
using UnityEngine;

public class PlayerSwordVisual : MonoBehaviour
{
    private PlayerAnimator playerAnimator;

    private Vector3 defaultSwordPosition;
    private Vector3 flippedSwordPosition;

    private bool lastFlipState;
    void Start()
    {
        playerAnimator = transform.parent.parent.parent.GetComponent<PlayerAnimator>();
        defaultSwordPosition = transform.localPosition;
        flippedSwordPosition = new Vector3(-defaultSwordPosition.x, defaultSwordPosition.y, defaultSwordPosition.z);

        lastFlipState = playerAnimator.isFlipped;
        SwordPositionCheck();
    }

    void Update()
    {
        if (playerAnimator.isFlipped != lastFlipState)
        {
            lastFlipState = playerAnimator.isFlipped;
            SwordPositionCheck();
        }
    }
    void SwordPositionCheck()
    {
        transform.localScale = new Vector3(playerAnimator.isFlipped ? -1 : 1, 1, 1);

        transform.localPosition = playerAnimator.isFlipped ? flippedSwordPosition : defaultSwordPosition;
    }
}

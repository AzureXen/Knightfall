using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : EntityMovement
{
    [Header("Keybindings")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode dodgeKey = KeyCode.Mouse3;

    private PlayerManager playerManager;

    [SerializeField] private PlayerDataSO playerData;

    [SerializeField] private Boolean canMove;
    public float moveSpeed;
    public float sprintSpeed;
    public float speedBoost = 1f;
    public float dodgeSpeed;
    Rigidbody2D rb;
    [HideInInspector] public Vector2 moveDir;


    [SerializeField] private float staminaRegenPS;
    [SerializeField] private float staminaRegenBonusMultiplier;
    [SerializeField] private float sprintCostPerSecond;
    [SerializeField] private float dodgeCost;
    [SerializeField] private float dodgeIFrameDuration;


    public Boolean isRunning;
    public Boolean isHoldingDodge;
    public Boolean isHoldingSprint;
    public Boolean isWalking;

    private Camera mainCam;
    private Vector3 mousePos;
    private Vector2 defaultDodgeDirection;
    private Boolean isDodging = false;
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        canMove = true;
        playerManager = GetComponent<PlayerManager>();
    }

    
    void Update()
    {
        InputManager();   
    }

    private void FixedUpdate()
    {
        Move();
        Run();
        RegenStaminaPerSecond();
    }

    void InputManager()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(moveX, moveY).normalized;
        isWalking = moveDir != Vector2.zero;

        // After dodging - dashing, if player continues to hold sprint key, player will start sprinting
        if (Input.GetKey(dodgeKey))
        {
            isHoldingDodge = true;
        }
        else isHoldingDodge = false;

        // Sprint without dodging - dashing
        if (Input.GetKey(sprintKey))
        {
            isHoldingSprint = true;
        }
        else isHoldingSprint = false;

        // Get mouse position, to get the default dodge direction
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // The default dodge direction is backwards, away from the mouse
        defaultDodgeDirection = -(mousePos - transform.position).normalized;

        // Dodging
        if (Input.GetKeyDown(dodgeKey) && !isDodging && canMove)
        {
            if (canMove && playerData.Stamina > 0)
            {
                playerData.DecreaseStamina(dodgeCost);
                StartCoroutine(Dodge());
            }
        }
    }

    void Move()
    {
        if (!canMove || isDodging) return;

        if (isRunning)
        {
            rb.linearVelocity = moveDir * sprintSpeed * speedBoost;
            if (playerData.Stamina > 0)
            {
                playerData.DecreaseStamina(sprintCostPerSecond * Time.deltaTime);
                Mathf.Clamp(playerData.Stamina, 0, playerData.MaxStamina);
            }
        }
        else
        {
            rb.linearVelocity = moveDir * moveSpeed * speedBoost;
        }
    }

    void Run()
    {
        isRunning = isHoldingSprint || isHoldingDodge;
    }

    public IEnumerator Dodge()
    {
        isDodging = true;

        playerManager.TakeIFrameNoCollision(dodgeIFrameDuration);

        DisableMovement();

        Vector2 dodgeDir = moveDir != Vector2.zero ? moveDir : defaultDodgeDirection;

        rb.linearVelocity = dodgeDir.normalized * dodgeSpeed;

        yield return new WaitForSeconds(0.2f);

        rb.linearVelocity *= 0.5f;

        EnableMovement();
        isDodging = false;
    }

    void RegenStaminaPerSecond()
    {
        if(playerData.Stamina < playerData.MaxStamina && !isRunning)
        {
            if (!isWalking)
            {
                playerData.IncreaseStamina(staminaRegenPS * staminaRegenBonusMultiplier * Time.deltaTime);
            }
            else playerData.IncreaseStamina(staminaRegenPS * Time.deltaTime);
        }
        Mathf.Clamp(playerData.Stamina, 0 , playerData.MaxStamina);
    }
    public override void DisableMovement()
    {
        canMove = false;
    }
    public override void EnableMovement()
    {
        canMove = true;
    }
}

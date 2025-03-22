using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : EntityMovement
{

    private PlayerManager playerManager;

    [SerializeField] private Boolean canMove;
    public float moveSpeed;
    public float sprintSpeed;
    public float dodgeSpeed;
    Rigidbody2D rb;
    [HideInInspector] public Vector2 moveDir;

    public float maxStamina;
    public float stamina;
    [SerializeField] private float staminaRegenPS;
    [SerializeField] private float staminaRegenBonusMultiplier;
    [SerializeField] private float sprintCostPerSecond;
    [SerializeField] private float dodgeCost;
    [SerializeField] private float dodgeIFrameDuration;


    public Boolean isRunning;
    public Boolean isHoldingShift;
    public Boolean isHoldingCtrl;
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

        // After dodging - dashing, if player continues to hold shift, player will start sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isHoldingShift = true;
        }
        else isHoldingShift = false;

        // Sprint without dodging - dashing
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isHoldingCtrl = true;
        }
        else isHoldingCtrl = false;

        // Get mouse position, to get the default dodge direction

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // The default dodge direction is backwards, away from the mouse

        defaultDodgeDirection = -(mousePos - transform.position).normalized;

        // Dodging
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDodging && canMove)
        {
            if (canMove && stamina > 0)
            {
                stamina -= dodgeCost;
                StartCoroutine(Dodge());
            }
        }
    }

    void Move()
    {
        if (!canMove || isDodging) return;

        if (isRunning)
        {
            rb.linearVelocity = moveDir * sprintSpeed;
            if (stamina > 0)
            {
                stamina -= sprintCostPerSecond * Time.deltaTime;
                Mathf.Clamp(stamina, 0, maxStamina);
            }
        }
        else
        {
            rb.linearVelocity = moveDir * moveSpeed;
        }
    }

    void Run()
    {
        isRunning = isHoldingShift || isHoldingCtrl;
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
        if(stamina < maxStamina && !isRunning)
        {
            if (!isWalking)
            {
                stamina += staminaRegenPS * staminaRegenBonusMultiplier * Time.deltaTime;
            }
            else stamina += staminaRegenPS * Time.deltaTime;
        }
        Mathf.Clamp(stamina, 0 , maxStamina);
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

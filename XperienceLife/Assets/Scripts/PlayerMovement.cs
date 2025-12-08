using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D playerRB;

    [Header("Movement")]
    [SerializeField] int maxSpeed = 5;               // base walk speed
    [SerializeField] float sprintMultiplier = 1.75f; // how much faster sprint is

    // Sprint / stamina
    [Header("Stamina / Sprint")]
    [SerializeField] float staminaDrainPerSecond = 20f;   // how fast stamina drains while sprinting
    [SerializeField] float staminaRegenPerSecond = 15f;   // how fast stamina comes back when not sprinting
    [SerializeField] float minStaminaToSprint = 5f;       // need at least this to start sprinting

    [Header("Combat")]
    [SerializeField] private PlayerMeleeAttack meleeAttack;


    // Stores current movement from input system (X = horizontal, Y = vertical)
    Vector2 moveInput;
    Vector2 lastMoveInput = Vector2.right;

    // Sprint input
    bool sprintHeld = false;
    bool isSprinting = false;

    // Generated input actions class
    InputSystem_Actions controls;

    // Stats reference
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Aim Indicator")]
    [SerializeField] Transform aimIndicator;
    [SerializeField] float aimDistance = 0.7f;

    public Vector2 AimDirection => lastMoveInput.normalized;

    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        controls = new InputSystem_Actions();

        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();
    }

    void OnEnable()
    {
        controls.Enable();

        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;

        // Sprint toggles
        controls.Player.Sprint.performed += OnSprintPerformed;
        controls.Player.Sprint.canceled += OnSprintCanceled;

        controls.Player.Attack.performed += OnAttack;
    }

    void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;

        controls.Player.Sprint.performed -= OnSprintPerformed;
        controls.Player.Sprint.canceled -= OnSprintCanceled;

        controls.Player.Attack.performed -= OnAttack;

        controls.Disable();
    }

    // New Input System callback (Move)
    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 0.001f)
        {
            lastMoveInput = moveInput;
        }
    }

    // For PlayerInput "Send Messages" pattern (if you're using it)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        if (moveInput.sqrMagnitude > 0.001f)
        {
            lastMoveInput = moveInput;
        }
    }

    // Sprint callbacks
    private void OnSprintPerformed(InputAction.CallbackContext ctx)
    {
        sprintHeld = ctx.ReadValueAsButton();
    }

    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        sprintHeld = false;
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void Update()
    {
        UpdateSprintStamina();
        UpdateAimIndicatorRotation();
    }

    private void UpdateMovement()
    {
        float speed = maxSpeed;

        // Decide if we can sprint this frame
        bool wantsToSprint = sprintHeld && moveInput.sqrMagnitude > 0.001f;
        bool hasStamina = playerStats != null && playerStats.currentStamina > minStaminaToSprint;

        isSprinting = wantsToSprint && hasStamina;

        if (isSprinting)
        {
            speed *= sprintMultiplier;
        }

        // Apply movement
        // If your Rigidbody2D uses velocity instead of linearVelocity, swap this line.
        playerRB.linearVelocity = moveInput * speed;
    }

    private void UpdateSprintStamina()
    {
        if (playerStats == null)
            return;

        bool isMoving = moveInput.sqrMagnitude > 0.001f;

        if (isSprinting && isMoving)
        {
            // Drain stamina while sprinting and moving
            playerStats.currentStamina -= staminaDrainPerSecond * Time.deltaTime;

            if (playerStats.currentStamina <= 0f)
            {
                playerStats.currentStamina = 0f;
                isSprinting = false;         // hard stop sprinting
            }
        }
        else
        {
            // Only regen if player is NOT holding sprint
            if (!sprintHeld && playerStats.currentStamina < playerStats.stamina)
            {
                playerStats.currentStamina += staminaRegenPerSecond * Time.deltaTime;
                if (playerStats.currentStamina > playerStats.stamina)
                    playerStats.currentStamina = playerStats.stamina;
            }
        }
    }


    private void UpdateAimIndicatorRotation()
    {
        if (aimIndicator == null || lastMoveInput.sqrMagnitude < 0.001f)
        {
            return;
        }

        Vector2 dir = lastMoveInput.normalized;
        aimIndicator.localPosition = dir * aimDistance;

        float angle = Mathf.Atan2(lastMoveInput.y, lastMoveInput.x) * Mathf.Rad2Deg;
        aimIndicator.localRotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (meleeAttack != null)
        {
            meleeAttack.PerformAttack();
        }
    }

}

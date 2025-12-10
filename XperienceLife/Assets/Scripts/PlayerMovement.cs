using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D playerRB;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.75f;

    [Header("Stamina / Sprint")]
    [SerializeField] private float staminaDrainPerSecond = 20f;   // drain while sprinting
    [SerializeField] private float staminaRegenPerSecond = 15f;   // regen when not sprinting
    [SerializeField] private float minStaminaToSprint = 5f;       // need at least this to start sprinting

    [Header("Aim Indicator")]
    [SerializeField] private Transform aimIndicator;
    [SerializeField] private float aimDistance = 0.7f;

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    private InputSystem_Actions controls;
    private Vector2 moveInput;
    private Vector2 lastMoveInput = Vector2.right;

    private bool sprintHeld = false;
    private bool isSprinting = false;

    public Vector2 AimDirection => lastMoveInput.normalized;
    public bool IsSprinting => isSprinting;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        controls = new InputSystem_Actions();

        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();
    }

    private void OnEnable()
    {
        controls.Enable();

        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled  += OnMove;

        controls.Player.Sprint.performed += OnSprintPerformed;
        controls.Player.Sprint.canceled  += OnSprintCanceled;
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled  -= OnMove;

        controls.Player.Sprint.performed -= OnSprintPerformed;
        controls.Player.Sprint.canceled  -= OnSprintCanceled;

        controls.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 0.001f)
            lastMoveInput = moveInput;
    }

    private void OnSprintPerformed(InputAction.CallbackContext ctx)
    {
        sprintHeld = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        sprintHeld = false;
    }

    private void FixedUpdate()
    {
        HandleMovementAndStamina();
    }

    private void Update()
    {
        UpdateAimIndicator();
    }

    private void HandleMovementAndStamina()
    {
        float targetSpeed = walkSpeed;
        bool hasMoveInput = moveInput.sqrMagnitude > 0.001f;

        if (playerStats != null)
        {
            // -----------------------------
            // Start / Stop Sprinting Logic
            // -----------------------------

            // Try to START sprinting only if enough stamina
            if (!isSprinting &&
                sprintHeld &&
                hasMoveInput &&
                playerStats.currentStamina >= minStaminaToSprint)
            {
                isSprinting = true;
            }

            // Stop sprinting if sprint released or stamina empty
            if (!sprintHeld || playerStats.currentStamina <= 0f)
            {
                isSprinting = false;
            }

            // -----------------------------
            // Sprint Active → Drain stamina
            // -----------------------------
            if (isSprinting)
            {
                targetSpeed = walkSpeed * sprintMultiplier;

                float drain = staminaDrainPerSecond * Time.fixedDeltaTime;
                playerStats.currentStamina -= drain;

                if (playerStats.currentStamina <= 0f)
                    playerStats.currentStamina = 0f;
            }
            else
            {
                // -----------------------------
                // Not sprinting → Regen stamina
                // -----------------------------
                if (playerStats.currentStamina < playerStats.maxStamina && !sprintHeld)
                {
                    float regen = staminaRegenPerSecond * Time.fixedDeltaTime;
                    playerStats.currentStamina += regen;

                    if (playerStats.currentStamina > playerStats.maxStamina)
                        playerStats.currentStamina = playerStats.maxStamina;
                }
            }
        }

        // Apply movement
        playerRB.linearVelocity = moveInput * targetSpeed;
    }

    private void UpdateAimIndicator()
    {
        if (aimIndicator == null || lastMoveInput.sqrMagnitude < 0.001f)
            return;

        Vector2 dir = lastMoveInput.normalized;

        aimIndicator.localPosition = dir * aimDistance;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        aimIndicator.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 0.8f;
    [SerializeField] private float idleSpeedThreshold = 0.05f;

    [Header("Visual")]
    [SerializeField] private Transform visual; // sprite parent to flip; if null, auto-assigned

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    private Vector2 movement = Vector2.zero;
    private bool isAttacking = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Try to find Animator in several ways
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            anim = GetComponent<Animator>();
        if (anim == null)
            anim = GetComponentInParent<Animator>();

        if (anim == null)
        {
            Debug.LogError($"EnemyMovement on '{name}' could not find an Animator. " +
                           $"Please add an Animator component or assign it manually.");
        }

        // Visual transform for flipping
        if (visual == null)
        {
            if (anim != null)
                visual = anim.transform;
            else
                visual = transform;
        }
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        // If no animator, just move and bail out of animation logic
        if (anim == null)
            return;

        if (player == null)
        {
            anim.SetFloat("Speed", 0f);
            anim.SetBool("IsWalkHorizontal", false);
            anim.SetBool("IsWalkVertical", false);
            return;
        }

        if (isAttacking)
        {
            movement = Vector2.zero;
            anim.SetFloat("Speed", 0f);
            anim.SetBool("IsWalkHorizontal", false);
            anim.SetBool("IsWalkVertical", false);
            return;
        }

        // Direction to player
        Vector2 toPlayer = (player.position - transform.position);
        float distance = toPlayer.magnitude;
        Vector2 dir = distance > 0.001f ? toPlayer / distance : Vector2.zero;

        if (distance > stopDistance)
            movement = dir;
        else
            movement = Vector2.zero;

        // ----- ANIMATOR PARAMS -----
        float speed = movement.magnitude;
        anim.SetFloat("Speed", speed);

        bool walkHori = false;
        bool walkVert = false;

        if (speed > idleSpeedThreshold)
        {
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
                walkHori = true;
            else
                walkVert = true;

            // Flip sprite when going left/right
            if (movement.x < -0.05f)
            {
                visual.localScale = new Vector3(
                    -Mathf.Abs(visual.localScale.x),
                    visual.localScale.y,
                    visual.localScale.z
                );
            }
            else if (movement.x > 0.05f)
            {
                visual.localScale = new Vector3(
                    Mathf.Abs(visual.localScale.x),
                    visual.localScale.y,
                    visual.localScale.z
                );
            }
        }

        anim.SetBool("IsWalkHorizontal", walkHori);
        anim.SetBool("IsWalkVertical", walkVert);
    }

    private void FixedUpdate()
    {
        if (isAttacking)
            rb.linearVelocity = Vector2.zero;
        else
            rb.linearVelocity = movement * moveSpeed;
    }

    // Called by EnemyMeleeAttack when an attack starts
    public void StartAttackAnimation()
    {
        if (anim == null) return;

        isAttacking = true;
        anim.SetBool("IsAttacking", true);
        anim.SetBool("IsWalkHorizontal", false);
        anim.SetBool("IsWalkVertical", false);

        // If you don't want any “pre-swing” frames, start a bit into the clip:
        // 0.0f = very start, 1.0f = very end
        float normalizedStartTime = 0.15f;  // tweak this (0.1–0.25)
        anim.Play("zomSwipe", 0, normalizedStartTime);
    }


    // Called by Animation Event at end of zomSwipe animation
    public void EndAttackAnimation()
    {
        if (anim == null) return;

        isAttacking = false;
        anim.SetBool("IsAttacking", false);
    }
}

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float idleSpeedThreshold = 0.05f;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Flipping")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private bool flipOnX = true;

    [Header("Weapon Visual (optional)")]
    [SerializeField] private SpriteRenderer weaponVisual;

    // Animator parameter hashes - MATCH Animator window
    private const string walkHorizontalBool = "WalkHori";
    private const string walkVerticalBool = "WalkVert";
    private const string isAttackingBool = "Attack";
    private const string isCastingBool = "Spell";

    private int facing = 1; // 1 = right, -1 = left

    public bool IsCasting
    {
        get
        {
            if (animator == null) return false;
            return animator.GetBool(isCastingBool); // reads "Spell"
        }
    }

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (rb == null)
            rb = GetComponentInParent<Rigidbody2D>();

        if (visualRoot == null)
            visualRoot = transform;

        if (weaponVisual == null)
            weaponVisual = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (rb == null || animator == null)
            return;

        Vector2 v = rb.linearVelocity;
        float speed = v.magnitude;

        bool isMoving = speed > idleSpeedThreshold;

        bool walkH = false;
        bool walkV = false;

        if (isMoving)
        {
            if (Mathf.Abs(v.x) >= Mathf.Abs(v.y))
                walkH = true;
            else
                walkV = true;
        }

        animator.SetBool(walkHorizontalBool, walkH);
        animator.SetBool(walkVerticalBool, walkV);

        HandleFlip(v);
    }

    private void HandleFlip(Vector2 velocity)
    {
        if (!flipOnX || visualRoot == null)
            return;

        if (Mathf.Abs(velocity.x) > 0.05f)
        {
            int newFacing = (velocity.x > 0f) ? 1 : -1;
            if (newFacing != facing)
            {
                facing = newFacing;
                Vector3 scale = visualRoot.localScale;
                scale.x = facing;
                visualRoot.localScale = scale;
            }
        }
    }

    // Called by PlayerMeleeAttack
    public void PlayAttack()
    {
        if (animator == null) return;

        // cancel casting if somehow active
        animator.SetBool(isCastingBool, false); // Spell = false

        if (weaponVisual != null)
            weaponVisual.enabled = true;

        animator.SetBool(isAttackingBool, true); // Attack = true
    }

    // Called by PlayerSpells
    public void PlaySpell()
    {
        if (animator == null) return;

        // cancel attack if somehow active
        animator.SetBool(isAttackingBool, false); // Attack = false

        if (weaponVisual != null)
            weaponVisual.enabled = false;

        animator.SetBool(isCastingBool, true); // Spell = true
    }

    // Animation Event at end of Attack clip
    public void OnAttackFinished()
    {
        animator.SetBool(isAttackingBool, false); // Attack = false
    }

    // Animation Event at end of Spell clip
    public void OnSpellFinished()
    {
        animator.SetBool(isCastingBool, false); // Spell = false

        if (weaponVisual != null)
            weaponVisual.enabled = true;
    }
}

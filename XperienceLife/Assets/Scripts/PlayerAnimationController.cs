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

    [Header("Combat References")]
    [SerializeField] private PlayerMeleeAttack meleeAttack;
    [SerializeField] private PlayerSpells playerSpells;

    // Animator parameter names – must match the Animator
    private const string WalkHoriParam = "WalkHori";
    private const string WalkVertParam = "WalkVert";
    private const string AttackParam   = "Attack";
    private const string SpellParam    = "Spell";

    private int facing = 1;

    public bool IsCasting =>
        animator != null && animator.GetBool(SpellParam);

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

        // make sure we can see the combat scripts
        if (meleeAttack == null)
            meleeAttack = GetComponentInParent<PlayerMeleeAttack>();

        if (playerSpells == null)
            playerSpells = GetComponentInParent<PlayerSpells>();
    }

    private void Update()
    {
        if (rb == null || animator == null)
            return;

        Vector2 v = rb.linearVelocity;
        float speed = v.magnitude;

        bool walkH = Mathf.Abs(v.x) > Mathf.Abs(v.y) && speed > idleSpeedThreshold;
        bool walkV = Mathf.Abs(v.y) >= Mathf.Abs(v.x) && speed > idleSpeedThreshold;

        animator.SetBool(WalkHoriParam, walkH);
        animator.SetBool(WalkVertParam, walkV);

        HandleFlip(v);
    }

    private void HandleFlip(Vector2 velocity)
    {
        if (!flipOnX || visualRoot == null)
            return;

        if (Mathf.Abs(velocity.x) > 0.05f)
        {
            int newFacing = velocity.x > 0f ? 1 : -1;
            if (newFacing != facing)
            {
                facing = newFacing;
                Vector3 scale = visualRoot.localScale;
                scale.x = facing;
                visualRoot.localScale = scale;
            }
        }
    }

    // ---------------------------
    // Called by scripts (input)
    // ---------------------------

    // PlayerMeleeAttack calls this to start the attack anim
    public void PlayAttack()
    {
        if (animator == null) return;

        animator.SetBool(SpellParam, false);
        animator.SetBool(AttackParam, true);

        if (weaponVisual != null)
            weaponVisual.enabled = true;

        Debug.Log("PlayAttack: Attack = TRUE");
    }

    // PlayerSpells calls this to start the spell anim
    public void PlaySpell()
    {
        if (animator == null) return;

        animator.SetBool(AttackParam, false);
        animator.SetBool(SpellParam, true);

        if (weaponVisual != null)
            weaponVisual.enabled = false;

        Debug.Log("PlaySpell: Spell = TRUE");
    }

    // ---------------------------
    // Called by animation events
    // ---------------------------

    // Event from end (or start) of attack anim to clear the bool
    public void OnAttackFinished()
    {
        if (animator == null) return;

        animator.SetBool(AttackParam, false);
        Debug.Log("OnAttackFinished: Attack = FALSE");

        if (weaponVisual != null)
            weaponVisual.enabled = true;
    }

    // Event from end (or start) of spell anim to clear the bool
    public void OnSpellFinished()
    {
        if (animator == null) return;

        animator.SetBool(SpellParam, false);
        Debug.Log("OnSpellFinished: Spell = FALSE");

        if (weaponVisual != null)
            weaponVisual.enabled = true;
    }

    // 🔹 Event to actually DO the melee hit (we’ll implement body next)
    public void Animation_MeleeHit()
    {
        Debug.Log("Animation_MeleeHit event");

        if (meleeAttack != null)
        {
            meleeAttack.Animation_MeleeHit();   // we’ll add this method on the melee script
        }
    }

    // 🔹 Event to actually CAST the spell (fireball) 
    public void Animation_SpellCast()
    {
        Debug.Log("Animation_SpellCast event");

        if (playerSpells != null)
        {
            playerSpells.Animation_FireballCast(); // we’ll add this method on the spell script
        }
    }
}

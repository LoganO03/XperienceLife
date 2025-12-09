using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputBridge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMeleeAttack melee;
    [SerializeField] private PlayerSpells spells;

    private InputSystem_Actions controls;

    private void Awake()
    {
        if (melee == null)
            melee = GetComponent<PlayerMeleeAttack>();

        if (spells == null)
            spells = GetComponent<PlayerSpells>();

        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controls.Enable();

        controls.Player.Attack.performed += OnAttackPerformed;
        controls.Player.Spell.performed += OnSpellPerformed;
    }

    private void OnDisable()
    {
        controls.Player.Attack.performed -= OnAttackPerformed;
        controls.Player.Spell.performed -= OnSpellPerformed;

        controls.Disable();
    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log("[InputBridge] Attack performed");
        if (melee != null)
            melee.PerformAttack();
    }

    private void OnSpellPerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log("[InputBridge] Spell performed");
        if (spells != null)
            spells.CastSpell();
    }
}

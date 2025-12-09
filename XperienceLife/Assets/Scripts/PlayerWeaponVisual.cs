using UnityEngine;

public class PlayerWeaponVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer weaponRenderer;

    private void Awake()
    {
        if (weaponRenderer == null)
            weaponRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void ShowWeapon()
    {
        if (weaponRenderer != null)
            weaponRenderer.enabled = true;
    }

    public void HideWeapon()
    {
        if (weaponRenderer != null)
            weaponRenderer.enabled = false;
    }
}

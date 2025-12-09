using UnityEngine;

public class PlayerAppearanceApplier : MonoBehaviour
{
    [Header("Body Parts (tinted with skinColor)")]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer headRenderer;
    [SerializeField] private SpriteRenderer handsRenderer;
    [SerializeField] private SpriteRenderer feetRenderer;

    [Header("Eyes")]
    [SerializeField] private SpriteRenderer irisRenderer;

    [Header("Clothing")]
    [SerializeField] private GameObject shirtObject;
    [SerializeField] private SpriteRenderer shirtRenderer;

    [SerializeField] private GameObject pantsObject;
    [SerializeField] private SpriteRenderer pantsRenderer;

    [SerializeField] private GameObject shoesObject;
    [SerializeField] private SpriteRenderer shoesRenderer;

    [Header("Hair")]
    [SerializeField] private SpriteRenderer hairRenderer;
    [Tooltip("Optional: one sprite per hair style index. If empty, we only tint whatever sprite is already there.")]
    [SerializeField] private Sprite[] hairStyleBaseSprites;

    [Header("Debug")]
    [SerializeField] private bool logOnApply = true;

    private CharacterAppearance appearance;

    private void Start()
    {
        if (PlayerProfile.Instance == null)
        {
            Debug.LogError("PlayerAppearanceApplier: No PlayerProfile.Instance found in scene.");
            enabled = false;
            return;
        }

        appearance = PlayerProfile.Instance.appearance;
        if (appearance == null)
        {
            Debug.LogError("PlayerAppearanceApplier: PlayerProfile.Instance.appearance is null.");
            enabled = false;
            return;
        }

        if (logOnApply)
        {
            Debug.Log($"[PlayerAppearanceApplier] Start(): skin={appearance.skinColor}, " +
                      $"hairColor={appearance.hairColor}, hairIndex={appearance.hairStyleIndex}");
        }

        ApplyAll();
    }

    // LateUpdate so we override any Animator shenanigans each frame
    private void LateUpdate()
    {
        if (appearance == null) return;

        ApplyHairOnly(); // keep hammering hair so Animator can't override it
    }

    private void ApplyAll()
    {
        // Skin
        if (bodyRenderer)  bodyRenderer.color  = appearance.skinColor;
        if (headRenderer)  headRenderer.color  = appearance.skinColor;
        if (handsRenderer) handsRenderer.color = appearance.skinColor;
        if (feetRenderer)  feetRenderer.color  = appearance.skinColor;

        // Eyes
        if (irisRenderer) irisRenderer.color = appearance.eyeColor;

        // Clothing
        if (shirtObject)  shirtObject.SetActive(appearance.hasShirt);
        if (shirtRenderer) shirtRenderer.color = appearance.shirtColor;

        if (pantsObject)  pantsObject.SetActive(appearance.hasPants);
        if (pantsRenderer) pantsRenderer.color = appearance.pantsColor;

        if (shoesObject)  shoesObject.SetActive(appearance.hasShoes);
        if (shoesRenderer) shoesRenderer.color = appearance.shoesColor;

        // Hair (color + sprite)
        ApplyHairOnly();
    }

    private void ApplyHairOnly()
    {
        if (!hairRenderer) return;

        // Always tint hair
        hairRenderer.color = appearance.hairColor;

        // Style sprite if we have an array
        if (hairStyleBaseSprites != null && hairStyleBaseSprites.Length > 0)
        {
            int idx = Mathf.Clamp(appearance.hairStyleIndex, 0, hairStyleBaseSprites.Length - 1);
            Sprite s = hairStyleBaseSprites[idx];

            if (s != null && hairRenderer.sprite != s)
            {
                hairRenderer.sprite = s;

                if (logOnApply)
                {
                    Debug.Log($"[PlayerAppearanceApplier] Applied hair style idx={idx}, sprite={s.name}");
                }
            }
        }
    }
}

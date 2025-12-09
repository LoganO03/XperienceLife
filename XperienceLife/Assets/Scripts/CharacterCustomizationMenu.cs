using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCustomizationMenu : MonoBehaviour
{
    [Header("Body Parts (tinted with skinColor)")]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer headRenderer;
    [SerializeField] private SpriteRenderer handsRenderer;
    [SerializeField] private SpriteRenderer feetRenderer;

    [Header("Eyes")]
    [Tooltip("Iris renderer only; eye whites stay white.")]
    [SerializeField] private SpriteRenderer irisRenderer;

    [Header("Clothing")]
    [SerializeField] private GameObject shirtObject;
    [SerializeField] private SpriteRenderer shirtRenderer;

    [SerializeField] private GameObject pantsObject;
    [SerializeField] private SpriteRenderer pantsRenderer;

    [SerializeField] private GameObject shoesObject;
    [SerializeField] private SpriteRenderer shoesRenderer;


    [Header("Clothing Toggles (equipped on/off)")]
    [SerializeField] private Toggle shirtToggle;
    [SerializeField] private Toggle pantsToggle;
    [SerializeField] private Toggle shoesToggle;

    [Header("Hair")]
    [SerializeField] private SpriteRenderer hairRenderer;
    [Tooltip("All available hair style sprites (0–3 for your 4 styles).")]
    [SerializeField] private Sprite[] hairStyles;

    [Header("Hair Style UI")]
    [SerializeField] private TextMeshProUGUI hairStyleLabel;
    [SerializeField] private bool hairLabelOneBased = true;

    [Header("Shared Color Panel UI")]
    [Tooltip("Panel that covers the screen when picking a color.")]
    [SerializeField] private GameObject colorPanel;
    [SerializeField] private Slider colorRSlider;
    [SerializeField] private Slider colorGSlider;
    [SerializeField] private Slider colorBSlider;
    [SerializeField] private TextMeshProUGUI colorTitleLabel;

    private enum ColorTarget
    {
        None,
        Skin,
        Eyes,
        Hair,
        Shirt,
        Pants,
        Shoes
    }

    private ColorTarget currentTarget = ColorTarget.None;

    private CharacterAppearance Appearance => PlayerProfile.Instance.appearance;

    private void Awake()
    {
        if (PlayerProfile.Instance == null)
        {
            Debug.LogError("CharacterCustomizationMenu: No PlayerProfile.Instance in scene. Add a PlayerProfile object.");
            enabled = false;
            return;
        }

        // Hide color panel at start
        if (colorPanel != null)
            colorPanel.SetActive(false);

        // Initialize clothing toggles from saved data
        if (shirtToggle != null) shirtToggle.isOn = Appearance.hasShirt;
        if (pantsToggle != null) pantsToggle.isOn = Appearance.hasPants;
        if (shoesToggle != null) shoesToggle.isOn = Appearance.hasShoes;

        ApplyAppearanceToRenderers();
        UpdateHairLabel();
    }

    // ------------ OPEN COLOR PANEL FOR EACH PART ------------

    public void OpenSkinColorPanel()
    {
        OpenColorPanel(ColorTarget.Skin, Appearance.skinColor, "Skin Color");
    }

    public void OpenEyeColorPanel()
    {
        OpenColorPanel(ColorTarget.Eyes, Appearance.eyeColor, "Eye Color");
    }

    public void OpenHairColorPanel()
    {
        OpenColorPanel(ColorTarget.Hair, Appearance.hairColor, "Hair Color");
    }

    public void OpenShirtColorPanel()
    {
        OpenColorPanel(ColorTarget.Shirt, Appearance.shirtColor, "Shirt Color");
    }

    public void OpenPantsColorPanel()
    {
        OpenColorPanel(ColorTarget.Pants, Appearance.pantsColor, "Pants Color");
    }

    public void OpenShoesColorPanel()
    {
        OpenColorPanel(ColorTarget.Shoes, Appearance.shoesColor, "Shoes Color");
    }

    private void OpenColorPanel(ColorTarget target, Color startColor, string title)
    {
        currentTarget = target;

        if (colorPanel != null)
            colorPanel.SetActive(true);

        if (colorTitleLabel != null)
            colorTitleLabel.text = title;

        if (colorRSlider != null) colorRSlider.value = startColor.r;
        if (colorGSlider != null) colorGSlider.value = startColor.g;
        if (colorBSlider != null) colorBSlider.value = startColor.b;
    }

    public void CloseColorPanel()
    {
        currentTarget = ColorTarget.None;
        if (colorPanel != null)
            colorPanel.SetActive(false);
    }

    // Hook this to R/G/B sliders OnValueChanged
    public void OnColorSliderChanged()
    {
        if (currentTarget == ColorTarget.None)
            return;

        float r = colorRSlider != null ? colorRSlider.value : 1f;
        float g = colorGSlider != null ? colorGSlider.value : 1f;
        float b = colorBSlider != null ? colorBSlider.value : 1f;

        Color newColor = new Color(r, g, b, 1f);
        ApplyColorToTarget(newColor);
        ApplyAppearanceToRenderers();
    }

    private void ApplyColorToTarget(Color c)
    {
        switch (currentTarget)
        {
            case ColorTarget.Skin:
                Appearance.skinColor = c;
                break;
            case ColorTarget.Eyes:
                Appearance.eyeColor = c;
                break;
            case ColorTarget.Hair:
                Appearance.hairColor = c;
                break;
            case ColorTarget.Shirt:
                Appearance.shirtColor = c;
                break;
            case ColorTarget.Pants:
                Appearance.pantsColor = c;
                break;
            case ColorTarget.Shoes:
                Appearance.shoesColor = c;
                break;
        }
    }

    // ------------ CLOTHING TOGGLES (EQUIPPED ON/OFF) ------------

    public void OnShirtToggleChanged(bool isOn)
    {
        Appearance.hasShirt = isOn;
        ApplyAppearanceToRenderers();
    }

    public void OnPantsToggleChanged(bool isOn)
    {
        Appearance.hasPants = isOn;
        ApplyAppearanceToRenderers();
    }

    public void OnShoesToggleChanged(bool isOn)
    {
        Appearance.hasShoes = isOn;
        ApplyAppearanceToRenderers();
    }




    // ------------ HAIR STYLE CONTROLS ------------

    public void NextHairStyle()
    {
        if (hairStyles == null || hairStyles.Length == 0)
            return;

        Appearance.hairStyleIndex++;
        if (Appearance.hairStyleIndex >= hairStyles.Length)
            Appearance.hairStyleIndex = 0;

        UpdateHairSprite();
        UpdateHairLabel();
    }

    public void PreviousHairStyle()
    {
        if (hairStyles == null || hairStyles.Length == 0)
            return;

        Appearance.hairStyleIndex--;
        if (Appearance.hairStyleIndex < 0)
            Appearance.hairStyleIndex = hairStyles.Length - 1;

        UpdateHairSprite();
        UpdateHairLabel();
    }

    public void OnConfirmPressed()
    {
        Debug.Log("Character appearance confirmed (stored in PlayerProfile.Instance.appearance).");
        // Example: UnityEngine.SceneManagement.SceneManager.LoadScene("Level 1");
    }

    public void OnRandomizePressed()
    {
        // Skin
        Appearance.skinColor = Random.ColorHSV(0f, 1f, 0.3f, 0.8f, 0.7f, 1f);

        // Eyes
        Appearance.eyeColor = Random.ColorHSV(0f, 1f, 0.4f, 1f, 0.4f, 1f);

        // Hair
        Appearance.hairColor = Random.ColorHSV(0f, 1f, 0.4f, 1f, 0.2f, 0.4f);

        // Clothing colors (don’t touch equipped flags here)
        Appearance.shirtColor = Random.ColorHSV(0f, 1f, 0.3f, 1f, 0.4f, 1f);
        Appearance.pantsColor = Random.ColorHSV(0f, 1f, 0.3f, 1f, 0.4f, 1f);
        Appearance.shoesColor = Random.ColorHSV(0f, 1f, 0.3f, 1f, 0.4f, 1f);

        if (hairStyles != null && hairStyles.Length > 0)
            Appearance.hairStyleIndex = Random.Range(0, hairStyles.Length);

        ApplyAppearanceToRenderers();
        UpdateHairLabel();
    }

    // ------------ APPLY TO SPRITES ------------

    private void ApplyAppearanceToRenderers()
    {
        // Skin
        if (bodyRenderer  != null) bodyRenderer.color  = Appearance.skinColor;
        if (headRenderer  != null) headRenderer.color  = Appearance.skinColor;
        if (handsRenderer != null) handsRenderer.color = Appearance.skinColor;
        if (feetRenderer  != null) feetRenderer.color  = Appearance.skinColor;

        // Eyes (iris)
        if (irisRenderer != null) irisRenderer.color = Appearance.eyeColor;

        // 🔹 Clothing visibility + color
        if (shirtObject != null)
            shirtObject.SetActive(Appearance.hasShirt);
        if (shirtRenderer != null)
            shirtRenderer.color = Appearance.shirtColor;

        if (pantsObject != null)
            pantsObject.SetActive(Appearance.hasPants);
        if (pantsRenderer != null)
            pantsRenderer.color = Appearance.pantsColor;

        if (shoesObject != null)
            shoesObject.SetActive(Appearance.hasShoes);
        if (shoesRenderer != null)
            shoesRenderer.color = Appearance.shoesColor;

        // Hair
        if (hairRenderer != null)
        {
            hairRenderer.color = Appearance.hairColor;
            UpdateHairSprite();
        }
    }


    private void UpdateHairSprite()
    {
        if (hairRenderer == null || hairStyles == null || hairStyles.Length == 0)
            return;

        int idx = Mathf.Clamp(Appearance.hairStyleIndex, 0, hairStyles.Length - 1);
        hairRenderer.sprite = hairStyles[idx];
    }

    private void UpdateHairLabel()
    {
        if (hairStyleLabel == null)
            return;

        int displayIndex = hairLabelOneBased
            ? Appearance.hairStyleIndex + 1
            : Appearance.hairStyleIndex;

        hairStyleLabel.text = $"Hair: {displayIndex}";
    }
}

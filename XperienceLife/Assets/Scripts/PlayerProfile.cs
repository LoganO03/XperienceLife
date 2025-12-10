using UnityEngine;

[System.Serializable]
public class CharacterAppearance
{
    // Skin-tinted parts: body, head, hands, feet
    public Color skinColor = new Color(1f, 0.86f, 0.72f, 1f);

    // Iris color (eye whites stay white)
    public Color eyeColor = new Color(0.2f, 0.4f, 0.8f, 1f);

    // Hair
    public Color hairColor = Color.black;
    public int hairStyleIndex = 0;   // later for multiple hair strips

    // Clothing colors
    public Color shirtColor = Color.white;
    public Color pantsColor = Color.gray;
    public Color shoesColor = Color.black;

    // Clothing equipped or not
    public bool hasShirt = true;
    public bool hasPants = true;
    public bool hasShoes = true;
}

public class PlayerProfile : MonoBehaviour
{
    public static PlayerProfile Instance { get; private set; }

    [Header("Character Appearance")]
    public CharacterAppearance appearance = new CharacterAppearance();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveProfile()
    {
        PlayerPrefs.SetFloat("skin_r", appearance.skinColor.r);
        PlayerPrefs.SetFloat("skin_g", appearance.skinColor.g);
        PlayerPrefs.SetFloat("skin_b", appearance.skinColor.b);

        PlayerPrefs.SetFloat("eye_r", appearance.eyeColor.r);
        PlayerPrefs.SetFloat("eye_g", appearance.eyeColor.g);
        PlayerPrefs.SetFloat("eye_b", appearance.eyeColor.b);

        PlayerPrefs.SetFloat("hair_r", appearance.hairColor.r);
        PlayerPrefs.SetFloat("hair_g", appearance.hairColor.g);
        PlayerPrefs.SetFloat("hair_b", appearance.hairColor.b);
        PlayerPrefs.SetInt("hair_style", appearance.hairStyleIndex);

        PlayerPrefs.SetFloat("shirt_r", appearance.shirtColor.r);
        PlayerPrefs.SetFloat("shirt_g", appearance.shirtColor.g);
        PlayerPrefs.SetFloat("shirt_b", appearance.shirtColor.b);
        PlayerPrefs.SetInt("has_shirt", appearance.hasShirt ? 1 : 0);

        PlayerPrefs.SetFloat("pants_r", appearance.pantsColor.r);
        PlayerPrefs.SetFloat("pants_g", appearance.pantsColor.g);
        PlayerPrefs.SetFloat("pants_b", appearance.pantsColor.b);
        PlayerPrefs.SetInt("has_pants", appearance.hasPants ? 1 : 0);

        PlayerPrefs.SetFloat("shoes_r", appearance.shoesColor.r);
        PlayerPrefs.SetFloat("shoes_g", appearance.shoesColor.g);
        PlayerPrefs.SetFloat("shoes_b", appearance.shoesColor.b);
        PlayerPrefs.SetInt("has_shoes", appearance.hasShoes ? 1 : 0);

        PlayerPrefs.Save();

        Debug.Log("Player Profile Saved!");
    }
}

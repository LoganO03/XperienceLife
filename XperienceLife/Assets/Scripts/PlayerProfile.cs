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
}

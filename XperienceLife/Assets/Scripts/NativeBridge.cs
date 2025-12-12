using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class StatBlock {
    public int health, stamina, strength, magic, intellect;
}

[System.Serializable]
public class AppearanceDTO {
    public float[] skinColor;
    public float[] eyeColor;
    public float[] hairColor;
    public int hairStyleIndex;
    public float[] shirtColor;
    public float[] pantsColor;
    public float[] shoesColor;
    public bool hasShirt, hasPants, hasShoes;
}

[System.Serializable]
public class LaunchConfig {
    public string scene;      // "CharacterCustomizerScene" or "Level1"
    public StatBlock stats;
    public AppearanceDTO appearance;
}

public class NativeBridge : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void StartGameWithConfig(string json)
    {
        var cfg = JsonUtility.FromJson<LaunchConfig>(json);
        if (cfg == null) {
            Debug.LogError("LaunchConfig JSON parse failed: " + json);
            return;
        }

        if (cfg.stats != null)
            ApplyStats(cfg.stats);

        if (cfg.appearance != null)
            ApplyAppearance(cfg.appearance);

        if (!string.IsNullOrEmpty(cfg.scene))
            SceneManager.LoadScene(cfg.scene);
    }

    void ApplyStats(StatBlock s)
    {
        var playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats == null) {
            Debug.LogWarning("PlayerStats not found in scene");
            return;
        }

        playerStats.health   = s.health;
        playerStats.stamina  = s.stamina;
        playerStats.strength = s.strength;
        playerStats.magic    = s.magic;
        playerStats.intellect= s.intellect;
    }

    void ApplyAppearance(AppearanceDTO a)
    {
        if (PlayerProfile.Instance == null) {
            Debug.LogWarning("PlayerProfile.Instance is null");
            return;
        }

        var ap = PlayerProfile.Instance.appearance;

        ap.skinColor   = ToColor(a.skinColor);
        ap.eyeColor    = ToColor(a.eyeColor);
        ap.hairColor   = ToColor(a.hairColor);
        ap.hairStyleIndex = a.hairStyleIndex;
        ap.shirtColor  = ToColor(a.shirtColor);
        ap.pantsColor  = ToColor(a.pantsColor);
        ap.shoesColor  = ToColor(a.shoesColor);
        ap.hasShirt    = a.hasShirt;
        ap.hasPants    = a.hasPants;
        ap.hasShoes    = a.hasShoes;

        PlayerProfile.Instance.SaveProfile();
    }

    Color ToColor(float[] rgb)
    {
        if (rgb == null || rgb.Length < 3)
            return Color.white;
        return new Color(rgb[0], rgb[1], rgb[2], 1f);
    }

    // Called from Confirm / Return buttons
    public void ReturnToNativeApp()
    {
        Application.Quit();
    }
}

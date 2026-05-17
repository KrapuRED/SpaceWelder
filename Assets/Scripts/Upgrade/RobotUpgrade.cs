using UnityEngine;

[CreateAssetMenu(fileName = "RobotUpgrade", menuName = "Robot/Upgrade Data")]
public class RobotUpgrade : ScriptableObject
{
    [Header("Max Limits — Edit these in Inspector")]
    public int maxBoomArm = 5;
    public float maxSpeed = 20f;
    public float maxWeldingArea = 5f;

    [Header("Default Values")]
    public int defaultExtraBoomArm = 0;
    public float defaultSpeed = 5f;
    public float defaultWeldingArea = 1f;

    // Runtime only — not serialized, never dirty the asset
    [System.NonSerialized] public int extraBoomArm;
    [System.NonSerialized] public float speed;
    [System.NonSerialized] public float weldingArea;

    public void Save()
    {
        PlayerPrefs.SetInt("extraBoomArm", extraBoomArm);
        PlayerPrefs.SetFloat("speed", speed);
        PlayerPrefs.SetFloat("weldingArea", weldingArea);
        PlayerPrefs.Save();
        Debug.Log("Upgrade saved!");
    }

    public void Load()
    {
        if (!PlayerPrefs.HasKey("extraBoomArm"))
        {
            Debug.Log("No save data found! Using defaults.");
            return; // keep whatever default values are set
        }

        extraBoomArm = PlayerPrefs.GetInt("extraBoomArm", defaultExtraBoomArm);
        speed = PlayerPrefs.GetFloat("speed", defaultSpeed);
        weldingArea = PlayerPrefs.GetFloat("weldingArea", defaultWeldingArea);
        Debug.Log($"Loaded! Arms:{extraBoomArm} Speed:{speed} Weld:{weldingArea}");
    }

    public void ResetUpgrade()
    {
        extraBoomArm = defaultExtraBoomArm;
        speed = defaultSpeed;
        weldingArea = defaultWeldingArea;
        Save();
    }
}

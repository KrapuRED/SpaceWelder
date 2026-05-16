using UnityEngine;

public class EffciencyShipManager : MonoBehaviour
{
    public static EffciencyShipManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        
    }
}

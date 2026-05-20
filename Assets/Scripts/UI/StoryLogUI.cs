using UnityEngine;
using TMPro;

public class StoryLogUI : MonoBehaviour
{
    [SerializeField] private TypeEffect typeEffectCargo;
    [SerializeField] private TypeEffect typeEffectClient;

    public void SetStoryLogUI(string cargo, string client)
    {
        if (this == null) return;

        typeEffectCargo.PlayText("cargo : " + cargo);
        typeEffectClient.PlayText("Client : " + client);
    }
}

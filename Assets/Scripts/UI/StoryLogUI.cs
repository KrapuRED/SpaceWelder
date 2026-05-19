using UnityEngine;
using TMPro;

public class StoryLogUI : MonoBehaviour
{
    public TextMeshProUGUI cargoText;
    public TextMeshProUGUI clienText;

    public void SetStoryLogUI(string cargo, string client)
    {
        if (this == null) return;

        cargoText.text = "Cargo : " + cargo;
        clienText.text = "Client : " + client;
    }
}

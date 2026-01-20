using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SessionManager : MonoBehaviour
{
    public GameObject setupCanvas;
    public GameObject gameCanvas;

    public TMP_Dropdown hospitalDropdown;
    public TMP_Dropdown modeDropdown;
    public TMP_Dropdown interactionDropdown;

    public NetworkManager networkManager;

    private Dictionary<string, string> hospitalMapping = new Dictionary<string, string>
    {
        { "Emergency Room", "ER" },
        { "Intensive Care Unit", "ICU" },
        { "Cardiology Department", "CARDIO" },
        { "Infectious Diseases Ward", "INFECT" },
        { "Pediatric Ward", "PEDS" },
        { "Surgery Ward", "SURGERY" }
    };

    public void OnGenerateButtonClicked()
    {
        string selectedDisplayText = hospitalDropdown.options[hospitalDropdown.value].text;

        string hospitalKey = hospitalMapping.ContainsKey(selectedDisplayText)
            ? hospitalMapping[selectedDisplayText]
            : selectedDisplayText;

        string mode = modeDropdown.options[modeDropdown.value].text.ToLower();
        string interaction = interactionDropdown.options[interactionDropdown.value].text.ToLower();

        networkManager.StartSession(hospitalKey, mode, interaction);

        setupCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }
}
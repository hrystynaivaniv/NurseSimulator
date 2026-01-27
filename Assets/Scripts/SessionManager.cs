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

    public MockNetworkManager networkManager;

    // Словник для мапінгу: Повна назва -> Короткий ключ
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
        // Отримуємо текст, який бачить користувач
        string selectedDisplayText = hospitalDropdown.options[hospitalDropdown.value].text;

        // Знаходимо короткий ключ у словнику. Якщо не знайдено — передаємо як є.
        string hospitalKey = hospitalMapping.ContainsKey(selectedDisplayText)
            ? hospitalMapping[selectedDisplayText]
            : selectedDisplayText;

        string mode = modeDropdown.options[modeDropdown.value].text.ToLower();
        string interaction = interactionDropdown.options[interactionDropdown.value].text.ToLower();

        // Передаємо короткий ключ "ER", "ICU" тощо на сервер
        networkManager.StartSession(hospitalKey, mode, interaction);

        setupCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }
}
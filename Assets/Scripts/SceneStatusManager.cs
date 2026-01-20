using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SceneStatusManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject vitalPanel;
    public GameObject inventoryPanel;
    public GameObject descriptionPanel;
    public GameObject dialoguePanel;

    [Header("UI Text Fields")]
    public TextMeshProUGUI painText;
    public TextMeshProUGUI tempText;
    public TextMeshProUGUI hrText;
    public TextMeshProUGUI bpText;
    public TextMeshProUGUI spo2Text;
    public TextMeshProUGUI rrText;

    [Header("Description & Inventory")]
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI dialogueTargetText;
    public TMP_Dropdown itemsDropdown;

    [Header("Standard Mode UI")]
    public GameObject standardActionsPanel;

    [Header("Special Buttons")]
    public GameObject descriptionButton;

    public void SetDialogueTarget(string role)
    {
        if (dialogueTargetText != null)
            dialogueTargetText.text = "Target: " + role;
    }

    public void OpenDialogueMode()
    {
        if (vitalPanel != null) vitalPanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (descriptionButton != null) descriptionButton.SetActive(false);
        if (descriptionPanel != null) descriptionPanel.SetActive(false);
        if (standardActionsPanel != null) standardActionsPanel.SetActive(false);
    }

    public void OpenDescriptionMode()
    {
        if (vitalPanel != null) vitalPanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    public void ReturnToGameMode()
    {
        if (vitalPanel != null) vitalPanel.SetActive(true);
        if (inventoryPanel != null) inventoryPanel.SetActive(true);
        if (descriptionButton != null) descriptionButton.SetActive(true);
        if (descriptionPanel != null) descriptionPanel.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public void UpdateValues(int pain, float temp, int hr, string bp, int spo2, int rr)
    {
        if (painText != null) painText.text = pain.ToString();
        if (tempText != null) tempText.text = temp.ToString("F1") + "°C";
        if (hrText != null) hrText.text = hr.ToString();
        if (bpText != null) bpText.text = bp;
        if (spo2Text != null) spo2Text.text = spo2.ToString() + "%";
        if (rrText != null) rrText.text = rr.ToString();
    }

    public void UpdateDescription(string text)
    {
        if (descriptionText != null) descriptionText.text = text;
    }

    public void UpdateInventory(List<InventoryItem> items)
    {
        if (itemsDropdown == null) return;
        itemsDropdown.ClearOptions();
        List<string> options = new List<string>();

        if (items != null)
        {
            foreach (var item in items)
            {
                if (item.count > 0)
                {
                    options.Add($"{item.name} (x{item.count})");
                }
            }
        }

        if (options.Count == 0) options.Add("Inventory Empty");
        itemsDropdown.AddOptions(options);
    }

    public void ToggleDescription()
    {
        if (descriptionPanel == null) return;
        bool isActive = descriptionPanel.activeSelf;

        if (!isActive)
        {
            descriptionPanel.SetActive(true);
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            OpenDescriptionMode();
        }
        else
        {
            ReturnToGameMode();
        }
    }
}
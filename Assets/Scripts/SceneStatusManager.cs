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

    [Header("Vital Signs Texts")]
    public TextMeshProUGUI painValueText;
    public TextMeshProUGUI tempValueText;
    public TextMeshProUGUI hrValueText;
    public TextMeshProUGUI bpValueText;
    public TextMeshProUGUI spo2ValueText;
    public TextMeshProUGUI rrValueText;

    [Header("Description & Inventory")]
    public TextMeshProUGUI descriptionText; 
    public TMP_Dropdown itemsDropdown;

    public void ToggleDescription()
    {
        if (descriptionPanel == null) return;

        bool isActive = descriptionPanel.activeSelf;

        if (!isActive)
        {
            descriptionPanel.SetActive(true);
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            OpenDialogueMode();
        }
        else
        {
            ReturnToGameMode();
        }
    }

    public void OpenDialogueMode()
    {
        if (vitalPanel != null) vitalPanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    public void OpenDescriptionMode()
    {
        if (descriptionPanel != null) descriptionPanel.SetActive(true);
        OpenDialogueMode();
    }

    public void ReturnToGameMode()
    {
        if (vitalPanel != null) vitalPanel.SetActive(true);
        if (inventoryPanel != null) inventoryPanel.SetActive(true);

        if (descriptionPanel != null) descriptionPanel.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public void UpdateValues(string pain, string temp, string hr, string bp, string spo2, string rr)
    {
        if (painValueText != null) painValueText.text = pain;
        if (tempValueText != null) tempValueText.text = temp;
        if (hrValueText != null) hrValueText.text = hr;
        if (bpValueText != null) bpValueText.text = bp;
        if (spo2ValueText != null) spo2ValueText.text = spo2;
        if (rrValueText != null) rrValueText.text = rr;
    }

    public void UpdateDescription(string text)
    {
        if (descriptionText != null) descriptionText.text = text;
    }

    public void UpdateInventory(string[] items)
    {
        if (itemsDropdown == null) return;
        itemsDropdown.ClearOptions();
        List<string> options = new List<string> { "Select Item" };
        if (items != null) options.AddRange(items);
        itemsDropdown.AddOptions(options);
    }
}


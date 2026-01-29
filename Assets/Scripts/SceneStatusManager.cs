using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class SceneStatusManager : MonoBehaviour
{
    [Header("Sliding Panel Settings")]
    public TextMeshProUGUI toggleButtonText;
    public RectTransform toggleButton;
    public RectTransform descriptionPanelRT;
    public float hiddenPosX = -95.7f;
    public float visiblePosX = 118f;

    [Header("UI Panels")]
    public GameObject vitalPanel;
    public GameObject inventoryPanel;
    public GameObject situationPanel;
    public GameObject dialoguePanel;
    public GameObject environmentPanel;
    public GameObject findingsPanel;
    public GameObject actionHistoryPanel;

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
        if (situationPanel != null) situationPanel.SetActive(false);
        if (environmentPanel != null) environmentPanel.SetActive(false);
        if (findingsPanel != null) findingsPanel.SetActive(false);
        if (actionHistoryPanel != null) actionHistoryPanel.SetActive(false);
        if (standardActionsPanel != null) standardActionsPanel.SetActive(false);
    }

    public void OpenDescriptionMode()
    {
        if (vitalPanel != null) vitalPanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);

        isPanelVisible = false;
        ToggleDescriptionPanel();
    }

    public void ReturnToGameMode()
    {
        if (vitalPanel != null) vitalPanel.SetActive(true);
        if (inventoryPanel != null) inventoryPanel.SetActive(true);
        if (descriptionButton != null) descriptionButton.SetActive(true);
        if (situationPanel != null) situationPanel.SetActive(false);
        if (environmentPanel != null) environmentPanel.SetActive(false);
        if (findingsPanel != null) findingsPanel.SetActive(false);
        if (actionHistoryPanel != null) actionHistoryPanel.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public void UpdateValues(VitalsData vitals)
    {
        if (vitals == null) return;

        if (painText != null) painText.text = vitals.pain.ToString() + "/10";
        if (tempText != null) tempText.text = vitals.temp.ToString("F1") + "°C";
        if (hrText != null) hrText.text = vitals.hr.ToString();
        if (bpText != null) bpText.text = vitals.bp;
        if (spo2Text != null) spo2Text.text = vitals.spo2.ToString() + "%";
        if (rrText != null) rrText.text = vitals.rr.ToString();
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

    public void ToggleSituation()
    {
        if (situationPanel == null) return;
        bool isActive = situationPanel.activeSelf;

        if (!isActive)
        {
            OpenDescriptionMode();
            if (situationPanel != null) situationPanel.SetActive(true);
        }
        else
        {
            ReturnToGameMode();
        }
    }


    private bool isPanelVisible = true;

    public void ToggleDescriptionPanel()
    {
        isPanelVisible = !isPanelVisible;

        Vector2 newPos = descriptionPanelRT.anchoredPosition;

        newPos.x = isPanelVisible ? visiblePosX : hiddenPosX;

        descriptionPanelRT.anchoredPosition = newPos;
        
        if (toggleButtonText != null)
        {
            toggleButtonText.text = isPanelVisible ? "<" : ">";
        }
    }

    public void ToggleEnvironment()
    {
        if (environmentPanel == null) return;
        bool isActive = environmentPanel.activeSelf;

        if (!isActive)
        {
            OpenDescriptionMode();
            if (environmentPanel != null) environmentPanel.SetActive(true);
        }
        else
        {
            ReturnToGameMode();
        }
    }

    public void ToggleFindings()
    {
        if (findingsPanel == null) return;
        bool isActive = findingsPanel.activeSelf;

        if (!isActive)
        {
            OpenDescriptionMode();
            if (findingsPanel != null) findingsPanel.SetActive(true);
        }
        else
        {
            ReturnToGameMode();
        }
    }

    public void ToggleActionHistory()
    {
        if (actionHistoryPanel == null) return;
        bool isActive = actionHistoryPanel.activeSelf;

        if (!isActive)
        {
            OpenDescriptionMode();
            if (actionHistoryPanel != null) actionHistoryPanel.SetActive(false);
        }
        else
        {
            ReturnToGameMode();
        }
    }
}
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class NurseInterface : MonoBehaviour
{
    [Header("Connections")]
    public SceneStatusManager statusManager;

    [Header("Main Windows")]
    public GameObject dialogueWindow;
    public GameObject responseWindow;

    [Header("Text Elements")]
    public TMP_InputField inputField;
    public TMP_Text responseText;

    [Header("UI Panels")]
    public GameObject choicePanel;
    public GameObject inputPanel;

    [Header("GameOver UI")]
    public GameObject restartButton;

    private string currentTargetName;

    void Start()
    {
        if (statusManager != null && statusManager.itemsDropdown != null)
        {
            statusManager.itemsDropdown.onValueChanged.AddListener(OnInventoryItemSelected);
        }
    }

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (dialogueWindow.activeSelf || responseWindow.activeSelf) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                if (IsCharacter(clickedObject))
                {
                    currentTargetName = clickedObject.name;
                    NetworkManager net = Object.FindFirstObjectByType<NetworkManager>();
                    if (net != null) net.ResetDecisionTimer();

                    ShowChoiceMenu();
                }
            }
        }
    }

    bool IsCharacter(GameObject obj)
    {
        return obj.CompareTag("Patient") ||
               obj.name.ToLower().Contains("character") ||
               obj.name == "Doctor" ||
               obj.name == "Visitor";
    }

    void ShowChoiceMenu()
    {
        statusManager.OpenDialogueMode();

        dialogueWindow.SetActive(true);
        choicePanel.SetActive(true);
        inputPanel.SetActive(false);
        statusManager.SetDialogueTarget(currentTargetName);
    }

    public void SelectMode(string mode)
    {
        choicePanel.SetActive(false);

        NetworkManager net = Object.FindFirstObjectByType<NetworkManager>();
        string currentMode = net.GetCurrentMode();

        if (currentMode == "standard")
        {
            statusManager.standardActionsPanel.SetActive(true);
            inputPanel.SetActive(false);
        }
        else
        {
            inputPanel.SetActive(true);
            inputField.text = "";
            inputField.ActivateInputField();
            statusManager.standardActionsPanel.SetActive(false);
        }
    }

    public void SendStandardAction(string actionKey)
    {
        NetworkManager net = Object.FindFirstObjectByType<NetworkManager>();
        if (net != null)
        {
            net.SendPlayerAction(currentTargetName, actionKey);
        }

        statusManager.standardActionsPanel.SetActive(false);
        dialogueWindow.SetActive(false);
    }

    public void FinalSend()
    {
        string text = inputField.text;
        if (string.IsNullOrEmpty(text)) return;

        NetworkManager net = Object.FindFirstObjectByType<NetworkManager>();
        if (net == null) return;

        if (text.StartsWith("Apply "))
        {
            string itemName = text.Replace("Apply ", "").Split(new string[] { " to" }, System.StringSplitOptions.None)[0];

            if (!net.HasItem(itemName))
            {
                responseText.text = "Error: You don't have enough of this item!";
                return;
            }

            net.DecreaseItemCount(itemName);
        }

        net.SendPlayerAction(currentTargetName, text);
        CloseDialogue();
    }

    public void ShowResponse(ActionResponse response, string target)
    {
        statusManager.OpenDialogueMode();
        responseWindow.SetActive(true);

        string displayMessage = "<b>Target:</b> " + target + "\n\n" + response.textResponse;

        if (!string.IsNullOrEmpty(response.verdict))
            displayMessage += "\n\n<b>Verdict:</b> " + response.verdict;

        if (response.scoreDelta != 0)
            displayMessage += "\n<b>Score:</b> " + (response.scoreDelta > 0 ? "+" : "") + response.scoreDelta;

        if (response.isGameOver)
        {
            displayMessage += "\n\n<color=red><b>SIMULATION COMPLETED!</b></color>";

            if (restartButton != null) restartButton.SetActive(true);
        }

        responseText.text = displayMessage;
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void CloseResponse()
    {
        responseWindow.SetActive(false);
        statusManager.ReturnToGameMode();
    }

    public void CloseDialogue()
    {
        dialogueWindow.SetActive(false);
        if (!responseWindow.activeSelf)
            statusManager.ReturnToGameMode();
    }

    void OnInventoryItemSelected(int index)
    {
        string selectedText = statusManager.itemsDropdown.options[index].text;
        string cleanName = selectedText;

        if (selectedText.Contains(" (x"))
        {
            cleanName = selectedText.Split(" (x")[0];
        }

        if (inputPanel.activeSelf)
        {
            inputField.text = "Apply " + cleanName + " to " + currentTargetName;
        }
    }
}
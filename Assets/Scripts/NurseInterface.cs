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

    private string currentTargetName;
    private string currentActionType;

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
                if (hit.collider.CompareTag("Patient") || hit.collider.gameObject.name.Contains("character"))
                {
                    currentTargetName = hit.collider.gameObject.name;
                    ShowChoiceMenu();
                }
            }
        }
    }

    void ShowChoiceMenu()
    {
        dialogueWindow.SetActive(true);
        choicePanel.SetActive(true);
        inputPanel.SetActive(false);

        if (statusManager != null) statusManager.OpenDialogueMode();
    }

    public void SelectMode(string mode)
    {
        currentActionType = mode;
        choicePanel.SetActive(false);
        inputPanel.SetActive(true);
        inputField.ActivateInputField();
    }

    public void FinalSend()
    {
        string text = inputField.text;
        if (string.IsNullOrEmpty(text)) return;

        NetworkManager net = Object.FindFirstObjectByType<NetworkManager>();
        if (net != null)
        {
            net.SendPlayerAction(currentTargetName, currentActionType, text);
        }

        CloseDialogue();
    }

    public void ShowResponse(string responseMsg)
    {
        responseWindow.SetActive(true);
        responseText.text = responseMsg;

        if (statusManager != null) statusManager.OpenDialogueMode();
    }

    public void CloseResponse()
    {
        responseWindow.SetActive(false);

        if (statusManager != null) statusManager.ReturnToGameMode();
    }

    public void CloseDialogue()
    {
        dialogueWindow.SetActive(false);
        inputField.text = "";

        if (!responseWindow.activeSelf && statusManager != null)
            statusManager.ReturnToGameMode();
    }
}
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class NurseInterface : MonoBehaviour
{
    public SceneStatusManager statusManager;
    public MockNetworkManager networkManager;

    [Header("Windows")]
    public GameObject dialogueWindow;
    public GameObject responseWindow;
    public GameObject loadingInitialScreen;
    public GameObject awaitingResponseScreen;

    [Header("UI Panels")]
    public GameObject choicePanel;
    public GameObject inputPanel;

    [Header("Input")]
    public TMP_InputField inputField;
    public TMP_Text responseText;

    [Header("Buttons")]
    public GameObject restartButton;
    public GameObject closeResponseButton;

    private string currentTargetName;

    private void OnEnable()
    {
        if (networkManager != null)
        {
            networkManager.OnActionReceived += OnActionFinished;
            networkManager.OnErrorOccurred += HideLoadingAndShowError;
            networkManager.OnSessionStarted += (data) => CloseLoadingOnly();
            networkManager.OnLoadingStateChanged += HandleLoadingState;
        }
    }

    private void OnDisable()
    {
        if (networkManager != null)
        {
            networkManager.OnActionReceived -= OnActionFinished;
            networkManager.OnErrorOccurred -= HideLoadingAndShowError;
            networkManager.OnLoadingStateChanged -= HandleLoadingState;
        }
    }

    private void HandleLoadingState(bool isActive, string type)
    {
        if (isActive) this.gameObject.SetActive(true);

        if (isActive)
        {
            if (type == "initial") ShowInitialLoadingScreen();
            else ShowAwaitingResponseScreen();
        }
        else
        {
            CloseLoadingOnly();
        }

    }

    private void OnActionFinished(ActionResponse response, string target, int stepTime)
    {
        int totalTime = networkManager.SimCore.TotalTimeSec;
        ShowResponse(response, target, stepTime, totalTime);
    }

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

        if (inputField.isFocused) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (IsAnyUIActive()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                if (clickedObject.name == "Patient")
                {
                    currentTargetName = clickedObject.name;
                    networkManager.SimCore.ResetActionTimer();
                    OpenDialogue();
                }
            }
        }
    }

    private bool IsAnyUIActive()
    {
        return (dialogueWindow != null && dialogueWindow.activeSelf) ||
               (responseWindow != null && responseWindow.activeSelf) ||
               (loadingInitialScreen != null && loadingInitialScreen.activeSelf) ||
               (awaitingResponseScreen != null && awaitingResponseScreen.activeSelf);
    }

    public void ShowInitialLoadingScreen()
    {
        this.gameObject.SetActive(true);

        if (loadingInitialScreen != null) loadingInitialScreen.SetActive(true);
        if (dialogueWindow != null) dialogueWindow.SetActive(false);
        if (statusManager != null) statusManager.OpenDialogueMode();
    }

    public void ShowAwaitingResponseScreen()
    {
        if (awaitingResponseScreen != null) awaitingResponseScreen.SetActive(true);
        if (dialogueWindow != null) dialogueWindow.SetActive(false);
    }

    public void CloseLoadingOnly()
    {
        if (loadingInitialScreen != null) loadingInitialScreen.SetActive(false);
        if (awaitingResponseScreen != null) awaitingResponseScreen.SetActive(false);

        if (statusManager != null) statusManager.ReturnToGameMode();

        if (EventSystem.current != null) EventSystem.current.enabled = true;
    }

    public void HideLoadingAndShowError(string error)
    {
        CloseLoadingOnly();

        if (responseWindow != null)
        {
            responseWindow.SetActive(true);
            responseText.text = "<color=red>Помилка: " + error + "</color>";
            if (closeResponseButton != null) closeResponseButton.SetActive(true);
        }
    }

    void OpenDialogue()
    {
        if (statusManager != null) statusManager.OpenDialogueMode();
        if (dialogueWindow != null)
        {
            dialogueWindow.SetActive(true);
            choicePanel.SetActive(true);
            if (inputPanel != null) inputPanel.SetActive(false);
            statusManager.SetDialogueTarget(currentTargetName);
        }
    }

    public void SelectMode(string mode)
    {
        if (currentTargetName != "Patient") return;

        choicePanel.SetActive(false);
        string currentMode = networkManager.GetCurrentMode();

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

    public void FinalSend()
    {
        string text = inputField.text;
        if (string.IsNullOrEmpty(text)) return;

        inputField.DeactivateInputField();

        ShowAwaitingResponseScreen();
        networkManager.SendPlayerAction(currentTargetName, text);

        if (statusManager != null)
            statusManager.UpdateInventory(networkManager.SimCore.Inventory);
    }

    public void SendStandardAction(string actionKey)
    {
        ShowAwaitingResponseScreen();
        networkManager.SendPlayerAction(currentTargetName, actionKey);
        if (statusManager != null && statusManager.standardActionsPanel != null)
            statusManager.standardActionsPanel.SetActive(false);
        if (dialogueWindow != null) dialogueWindow.SetActive(false);
    }

    public void ShowResponse(ActionResponse response, string target, int stepTime, int totalTime)
    {
        if (awaitingResponseScreen != null) awaitingResponseScreen.SetActive(false);

        if (statusManager != null) statusManager.OpenDialogueMode();
        if (responseWindow != null) responseWindow.SetActive(true);

        string displayMessage = $"<b>Target:</b> {target}\n\n{response.textResponse}";

        if (!string.IsNullOrEmpty(response.verdict))
            displayMessage += $"\n\n<b>Result:</b> {response.verdict}";

        string scoreSign = response.scoreDelta > 0 ? "+" : "";

        displayMessage += $"\n\n<b>Score change:</b> {scoreSign}{response.scoreDelta} (Total: {response.totalScore})";
        displayMessage += $"\n<b>Progress:</b> Step {response.stepCount} of {response.maxSteps}";
        displayMessage += $"\n<b>Time spent:</b> {stepTime} sec";

        bool endOfGame = response.isGameOver || !response.isAlive;

        if (!response.isAlive) displayMessage += "\n\n<color=red><b>PATIENT IS DECEASED</b></color>";

        if (response.isGameOver)
        {
            displayMessage += "\n\n<color=red><b>SIMULATION COMPLETED!</b></color>";
            displayMessage += $"\n<color=yellow><b>Total Session Time: {FormatTime(totalTime)}</b></color>";
        }

        if (closeResponseButton != null) closeResponseButton.SetActive(!endOfGame);
        if (restartButton != null) restartButton.SetActive(endOfGame);

        responseText.text = displayMessage;
    }

    private string FormatTime(int seconds)
    {
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void CloseResponse()
    {
        if (responseWindow != null) responseWindow.SetActive(false);
        if (statusManager != null) statusManager.ReturnToGameMode();
    }

    public void CloseDialogue()
    {
        if (dialogueWindow != null) dialogueWindow.SetActive(false);
        if (responseWindow != null && !responseWindow.activeSelf)
        {
            if (statusManager != null) statusManager.ReturnToGameMode();
        }
    }

    void OnInventoryItemSelected(int index)
    {
        if (statusManager == null || statusManager.itemsDropdown == null) return;

        string selectedText = statusManager.itemsDropdown.options[index].text;
        string cleanName = selectedText;
        if (selectedText.Contains(" (x"))
            cleanName = selectedText.Split(" (x")[0];

        if (inputPanel != null && inputPanel.activeSelf)
            inputField.text = "Застосувати " + cleanName + " до " + currentTargetName;
    }
}
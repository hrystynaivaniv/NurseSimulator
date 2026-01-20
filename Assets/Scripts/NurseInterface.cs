using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class NurseInterface : MonoBehaviour
{
    public SceneStatusManager statusManager;
    public NetworkManager networkManager;

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
        else
        {
            Debug.LogError("NetworkManager is not assigned in NurseInterface!");
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
        if (isActive)
        {
            if (type == "initial") ShowInitialLoadingScreen();
            else ShowAwaitingResponseScreen();

            if (EventSystem.current != null) EventSystem.current.enabled = false;
        }
        else
        {
            if (loadingInitialScreen != null) loadingInitialScreen.SetActive(false);
            if (awaitingResponseScreen != null) awaitingResponseScreen.SetActive(false);

            if (EventSystem.current != null) EventSystem.current.enabled = true;
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
            if (dialogueWindow.activeSelf || responseWindow.activeSelf || loadingInitialScreen.activeSelf || awaitingResponseScreen.activeSelf) return;

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

    public void ShowInitialLoadingScreen()
    {
        if (loadingInitialScreen != null) loadingInitialScreen.SetActive(true);
        if (dialogueWindow != null) dialogueWindow.SetActive(false);
        if (statusManager != null) statusManager.OpenDialogueMode();
    }

    public void ShowAwaitingResponseScreen()
    {
        if (awaitingResponseScreen != null) awaitingResponseScreen.SetActive(true);
        if (dialogueWindow != null) dialogueWindow.SetActive(false);
        if (statusManager != null) statusManager.OpenDialogueMode();
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
        if (loadingInitialScreen != null) loadingInitialScreen.SetActive(false);
        if (awaitingResponseScreen != null) awaitingResponseScreen.SetActive(false);

        responseWindow.SetActive(true);
        responseText.text = "<color=red>" + error + "</color>";

        if (closeResponseButton != null) closeResponseButton.SetActive(true);
        if (EventSystem.current != null) EventSystem.current.enabled = true;
    }

    void OpenDialogue()
    {
        statusManager.OpenDialogueMode();
        dialogueWindow.SetActive(true);
        choicePanel.SetActive(true);
        inputPanel.SetActive(false);
        statusManager.SetDialogueTarget(currentTargetName);
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

    public void SendStandardAction(string actionKey)
    {
        if (EventSystem.current != null) EventSystem.current.enabled = false;

        ShowAwaitingResponseScreen();
        networkManager.SendPlayerAction(currentTargetName, actionKey);

        statusManager.standardActionsPanel.SetActive(false);
        dialogueWindow.SetActive(false);
    }

    public void FinalSend()
    {
        string text = inputField.text;
        if (string.IsNullOrEmpty(text)) return;

        ShowAwaitingResponseScreen();

        networkManager.SendPlayerAction(currentTargetName, text);

        statusManager.UpdateInventory(networkManager.SimCore.Inventory);

        CloseDialogue();
    }

    public void ShowResponse(ActionResponse response, string target, int stepTime, int totalTime)
    {
        if (awaitingResponseScreen != null) awaitingResponseScreen.SetActive(false);

        statusManager.OpenDialogueMode();
        responseWindow.SetActive(true);

        string displayMessage = $"<b>Target:</b> {target}\n\n{response.textResponse}";

        if (!string.IsNullOrEmpty(response.verdict))
            displayMessage += $"\n\n<b>Verdict:</b> {response.verdict}";

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
            cleanName = selectedText.Split(" (x")[0];

        if (inputPanel.activeSelf)
            inputField.text = "Apply " + cleanName + " to " + currentTargetName;
    }
}
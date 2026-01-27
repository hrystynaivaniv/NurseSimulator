using System;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [Header("Settings")]
    public string baseUrl = "https://be.medsim.staging.intela.cloud";
    public string username = "stage";
    public string password = "743XjxbXNLtTvvwa";

    public SimulationController SimCore;
    private WebClient _webClient;
    private string currentInteractionMode;

    void Awake()
    {
        SimCore = new SimulationController();
        _webClient = new WebClient(baseUrl, username, password);
    }

    public event Action<InitialSceneData> OnSessionStarted;
    public event Action<ActionResponse, string, int> OnActionReceived;
    public event Action<string> OnErrorOccurred;
    public event Action<bool, string> OnLoadingStateChanged;

    public void SetInteractionMode(string mode) => currentInteractionMode = mode;
    public string GetCurrentMode() => currentInteractionMode;


    public async void StartSession(string hospital, string mode, string interaction)
    {
        currentInteractionMode = interaction;
        SessionRequest req = new SessionRequest { hospitalType = hospital, mode = mode, interactionMode = interaction };
        string json = JsonUtility.ToJson(req);

        OnLoadingStateChanged?.Invoke(true, "initial");

        try
        {
            string res = await _webClient.PostAsync("/api/start", json);

            InitialSceneData data = JsonUtility.FromJson<InitialSceneData>(res);
            SimCore.InitializeSession(data);

            OnSessionStarted?.Invoke(data);
        }
        catch (Exception e)
        {
            OnErrorOccurred?.Invoke("Start Error: " + e.Message);
        }
        finally
        {
            OnLoadingStateChanged?.Invoke(false, "initial");
        }
    }

    public async void SendPlayerAction(string target, string message)
    {
        if (string.IsNullOrEmpty(SimCore.SessionId)) return;

        int stepTime = SimCore.CalculateStepTime();

        string finalContent = SimCore.PrepareActionContent(target, message, currentInteractionMode);

        string json = "{\"actionType\":\"" + currentInteractionMode + "\",\"content\":\"" + finalContent + "\",\"decisionTimeSec\":" + stepTime + "}";

        OnLoadingStateChanged?.Invoke(true, "awaiting");

        try
        {
            string res = await _webClient.PutAsync($"/api/interact/{SimCore.SessionId}", json);

            ActionResponse response = JsonUtility.FromJson<ActionResponse>(res);
            SimCore.ProcessResponse(response, stepTime);

            OnActionReceived?.Invoke(response, target, stepTime);
        }
        catch (Exception e)
        {
            OnErrorOccurred?.Invoke("Action Error: " + e.Message);
        }
        finally
        {
            OnLoadingStateChanged?.Invoke(false, "awaiting");
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    [Header("Settings")]
    public string baseUrl = "https://boston-temptable-socioeconomically.ngrok-free.dev";

    [Header("Connections")]
    public SceneStatusManager statusManager;
    public SceneManager sceneManager;
    public NurseInterface nurseInterface;

    private string currentSessionId;
    private float actionStartTime;
    private string currentInteractionMode = "freeform";
    private List<InventoryItem> currentInventory = new List<InventoryItem>();

    public void SetInteractionMode(string mode)
    {
        currentInteractionMode = mode;
    }

    public void StartSession(string hospital, string mode, string interaction)
    {
        currentInteractionMode = interaction;

        SessionRequest req = new SessionRequest
        {
            hospitalType = hospital,
            mode = mode,
            interactionMode = interaction
        };
        string json = JsonUtility.ToJson(req);

        StartCoroutine(PostRequest(baseUrl + "/api/start", json, (res) => {
            ProcessInitialData(res);
        }));
    }

    private void ProcessInitialData(string json)
    {
        InitialSceneData data = JsonUtility.FromJson<InitialSceneData>(json);
        currentSessionId = data.sessionId;
        currentInventory = data.inventory;

        statusManager.UpdateInventory(currentInventory);

        statusManager.UpdateValues(
            data.vitals.pain, data.vitals.temp, data.vitals.hr,
            data.vitals.bp, data.vitals.spo2, data.vitals.rr
        );

        statusManager.UpdateDescription(data.situationDescription);
        sceneManager.SpawnSceneFromData(data);
        ResetDecisionTimer();
    }

    public void ResetDecisionTimer() => actionStartTime = Time.time;

    public void SendPlayerAction(string target, string message)
    {
        if (string.IsNullOrEmpty(currentSessionId)) return;

        int decisionTime = Mathf.RoundToInt(Time.time - actionStartTime);
        string finalContent = message;

        if (currentInteractionMode == "freeform")
        {
            finalContent = "[" + target + "] " + message;
        }

        string jsonToSend = "{\"actionType\":\"" + currentInteractionMode + "\",\"content\":\"" + finalContent + "\",\"decisionTimeSec\":" + decisionTime + "}";
        string url = $"{baseUrl}/api/interact/{currentSessionId}";

        StartCoroutine(PutRequest(url, jsonToSend, (json) => {
            ActionResponse response = JsonUtility.FromJson<ActionResponse>(json);

            nurseInterface.ShowResponse(response, target);

            statusManager.UpdateValues(
                response.updatedVitals.pain,
                response.updatedVitals.temp,
                response.updatedVitals.hr,
                response.updatedVitals.bp,
                response.updatedVitals.spo2,
                response.updatedVitals.rr
            );

            ResetDecisionTimer();
        }));
    }

    IEnumerator PostRequest(string url, string json, System.Action<string> callback)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("ngrok-skip-browser-warning", "true");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            callback(request.downloadHandler.text);
    }

    IEnumerator PutRequest(string url, string json, System.Action<string> callback)
    {
        var request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("ngrok-skip-browser-warning", "true");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            callback(request.downloadHandler.text);
    }

    public void DecreaseItemCount(string itemName)
    {
        var item = currentInventory.Find(i => i.name == itemName);
        if (item != null && item.count > 0)
        {
            item.count--;
            statusManager.UpdateInventory(currentInventory);
        }
    }

    public bool HasItem(string itemName)
    {
        var item = currentInventory.Find(i => i.name == itemName);
        return item != null && item.count > 0;
    }

    public string GetCurrentMode()
    {
        return currentInteractionMode;
    }
}
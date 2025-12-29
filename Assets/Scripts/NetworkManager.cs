using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class InitialSceneData
{
    public string roomType;
    public string situationDescription;
    public bool hasDoctor;
    public string patientGender;
    public string[] visitors;
    public string[] inventory;  
    public VitalSigns vitals;   
}

[System.Serializable]
public class VitalSigns
{
    public string pain;
    public string temp;
    public string hr;
    public string bp;
    public string spo2;
    public string rr;

}

[System.Serializable]
public class ActionResponse
{
    public string textResponse;  
    public VitalSigns updatedVitals; 
}

public class NetworkManager : MonoBehaviour
{
    public string baseUrl = ""; 

    [Header("References")]
    public SceneManager sceneManager;
    public SceneStatusManager statusManager;
    public NurseInterface nurseInterface;

    void Start()
    {
        if (!string.IsNullOrEmpty(baseUrl))
        {
            LoadInitialScene();
        }
    }

    public void LoadInitialScene()
    {
        StartCoroutine(GetRequest(baseUrl + "/init", (json) => {
            InitialSceneData data = JsonUtility.FromJson<InitialSceneData>(json);

            statusManager.UpdateValues(data.vitals.pain, data.vitals.temp, data.vitals.hr, data.vitals.bp, data.vitals.spo2, data.vitals.rr);
            statusManager.UpdateInventory(data.inventory);
            statusManager.UpdateDescription(data.situationDescription);
        }));
    }


    public void SendPlayerAction(string target, string type, string message)
    {
        string jsonToSend = "{\"target\":\"" + target + "\", \"type\":\"" + type + "\", \"text\":\"" + message + "\"}";

        StartCoroutine(PostRequest(baseUrl + "/action", jsonToSend, (json) => {
            ActionResponse response = JsonUtility.FromJson<ActionResponse>(json);

            nurseInterface.ShowResponse(response.textResponse);

            if (response.updatedVitals != null)
            {
                statusManager.UpdateValues(response.updatedVitals.pain, response.updatedVitals.temp,
                    response.updatedVitals.hr, response.updatedVitals.bp,
                    response.updatedVitals.spo2, response.updatedVitals.rr);
            }
        }));
    }

    IEnumerator GetRequest(string url, System.Action<string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success) callback(request.downloadHandler.text);
        }
    }

    IEnumerator PostRequest(string url, string json, System.Action<string> callback)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success) callback(request.downloadHandler.text);
    }
}
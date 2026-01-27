using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Rendering.HDROutputUtils;

public class WebClient
{
    private readonly string _authHeader;
    private readonly string _baseUrl;

    public WebClient(string baseUrl, string username, string password)
    {
        _baseUrl = baseUrl;

        string auth = username + ":" + password;
        string encodedAuth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        _authHeader = "Basic " + encodedAuth;
    }
    public async Task<string> PostAsync(string endpoint, string json)
    {
        var request = new UnityWebRequest(_baseUrl + endpoint, "POST");
        return await SendRequestAsync(request, json);
    }

    public async Task<string> PutAsync(string endpoint, string json)
    {
        var request = new UnityWebRequest(_baseUrl + endpoint, "PUT");
        return await SendRequestAsync(request, json);
    }

    private async Task<string> SendRequestAsync(UnityWebRequest request, string json)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        request.SetRequestHeader("Authorization", _authHeader);

        request.timeout = 90;

        var operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            return request.downloadHandler.text;
        }
        else
        {
            throw new Exception($"Status code: {request.responseCode}.\nError: {request.error}");
        }
    }
}
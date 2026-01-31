using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class SessionManager : MonoBehaviour
{
    [SerializeField]
    private SetUpWindowController _setUpWindowController;
    [SerializeField]
    private MockNetworkManager networkManager;

    private void Awake()
    {
        _setUpWindowController.OnGenerateBtnClickedEvent += OnGenerateButtonClicked;
    }

    public void OnGenerateButtonClicked(string hospitalKey, string mode, string interaction)
    {
        networkManager.StartSession(hospitalKey, mode, interaction);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Використовуємо InitialSceneData як основний тип, щоб уникнути конфліктів
public class MockNetworkManager : MonoBehaviour
{
    [Header("Connections")]
    public SceneStatusManager statusManager;
    public SceneManager sceneManager;
    public NurseInterface nurseInterface;

    public class MockSimCore
    {
        public string SessionId = "test-123";
        public int TotalTimeSec = 120;
        public List<InventoryItem> Inventory = new List<InventoryItem>();
        public void ResetActionTimer() { }
        public void ResetDecisionTimer() { }
    }
    public MockSimCore SimCore = new MockSimCore();

    // Використовуємо InitialSceneData тут, щоб OnSessionStarted збігався з підпискою в NurseInterface
    public event Action<InitialSceneData> OnSessionStarted;
    public event Action<ActionResponse, string, int> OnActionReceived;
    public event Action<string> OnErrorOccurred;
    public event Action<bool, string> OnLoadingStateChanged;

    private string currentInteractionMode = "freeform";
    public string GetCurrentMode() => currentInteractionMode;

    public void StartSession(string hospital, string mode, string interaction)
    {
        currentInteractionMode = interaction;
        StartCoroutine(MockStartRoutine());
    }

    private IEnumerator MockStartRoutine()
    {
        OnLoadingStateChanged?.Invoke(true, "initial");

        yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 5f));

        // Створюємо дані відразу в типі InitialSceneData
        InitialSceneData data = new InitialSceneData
        {
            sessionId = "test-session-123",
            situationDescription = "Пацієнтка, 28 років. Скаржиться на гострий біль у животі та нудоту.",
            inventory = new List<InventoryItem> {
                new InventoryItem { name = "Bandage", count = 5 },
                new InventoryItem { name = "Painkiller", count = 2 }
            },
            vitals = new VitalsData
            {
                pain = 8,
                temp = 38.2f,
                hr = 115,
                bp = "140/90",
                spo2 = 95,
                rr = 24
            }
        };

        // ТУТ ВИПРАВЛЕННЯ: Тепер передаємо ОДИН об'єкт vitals
        if (statusManager != null)
        {
            statusManager.UpdateValues(data.vitals);
            statusManager.UpdateInventory(data.inventory);
            statusManager.UpdateDescription(data.situationDescription);
        }

        // Тепер типи збігаються, помилки CS1503 не буде
        if (sceneManager != null)
        {
            sceneManager.SpawnSceneFromData(data);
        }

        OnLoadingStateChanged?.Invoke(false, "initial");
        OnSessionStarted?.Invoke(data);
    }

    public void SendPlayerAction(string target, string message)
    {
        StartCoroutine(MockActionRoutine(target));
    }

    private IEnumerator MockActionRoutine(string target)
    {
        OnLoadingStateChanged?.Invoke(true, "awaiting");
        yield return new WaitForSeconds(2f);

        ActionResponse response = new ActionResponse
        {
            textResponse = "Ви надали допомогу. Показники пацієнтки почали вирівнюватися.",
            verdict = "Діагноз та дії вірні",
            scoreDelta = 20,
            totalScore = 20,
            stepCount = 1,
            maxSteps = 5,
            isAlive = true,
            isGameOver = false,
            updatedVitals = new VitalsData { pain = 5, temp = 38.0f, hr = 98, bp = "120/80", spo2 = 98, rr = 18 }
        };

        OnLoadingStateChanged?.Invoke(false, "awaiting");
        OnActionReceived?.Invoke(response, target, 20);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class SimulationController
{
    public string SessionId { get; private set; }
    public int TotalScore { get; private set; }
    public int TotalTimeSec { get; private set; }
    public List<InventoryItem> Inventory { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsPatientAlive { get; private set; } = true;

    private DateTime _actionStartTime;


    public string PrepareActionContent(string target, string rawInput, string interactionMode)
    {
        string processedText = rawInput;

        if (rawInput.StartsWith("Apply "))
        {
            string itemName = rawInput.Replace("Apply ", "").Split(new string[] { " to" }, System.StringSplitOptions.None)[0];
            UseItem(itemName);
        }

        if (interactionMode == "freeform" && !rawInput.StartsWith("["))
        {
            return $"[{target}] {rawInput}";
        }
        return rawInput;
    }

    public void InitializeSession(InitialSceneData data)
    {
        SessionId = data.sessionId;
        Inventory = data.inventory;
        TotalTimeSec = 0;
        TotalScore = 0;
        ResetActionTimer();
    }

    public void ResetActionTimer() => _actionStartTime = DateTime.UtcNow;

    public int CalculateStepTime()
    {
        return (int)(DateTime.UtcNow - _actionStartTime).TotalSeconds;
    }

    public void ProcessResponse(ActionResponse response, int stepTime)
    {
        TotalScore = response.totalScore;
        TotalTimeSec += stepTime;
        IsGameOver = response.isGameOver;
        IsPatientAlive = response.isAlive;
    }

    public int GetItemCount(string itemName)
    {
        if (Inventory == null) return 0;
        foreach (var item in Inventory)
        {
            if (item.name == itemName) return item.count;
        }
        return 0;
    }

    public void UseItem(string itemName)
    {
        var item = Inventory?.Find(i => i.name == itemName);
        if (item != null && item.count > 0)
        {
            item.count--;
        }
    }
}
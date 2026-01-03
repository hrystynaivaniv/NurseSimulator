using System;
using System.Collections.Generic;

[Serializable]
public class InventoryItem
{
    public string name;
    public int count;
}

[Serializable]
public class VisitorData
{
    public string gender;
    public string role;
}


[Serializable]
public class VitalsData
{
    public int pain;
    public float temp;
    public int hr;
    public string bp;
    public int spo2;
    public int rr;
}

[Serializable]
public class InitialSceneData
{
    public string sessionId;
    public string roomType;
    public string situationDescription;
    public bool hasDoctor;
    public string patientGender;
    public List<VisitorData> visitors; // Використовуємо List
    public List<InventoryItem> inventory; // Використовуємо List
    public VitalsData vitals;
}

[Serializable]
public class ActionResponse
{
    public string textResponse;
    public VitalsData updatedVitals;
    public bool isAlive;
    public string verdict;
    public int scoreDelta;
    public bool isGameOver;
}

[Serializable]
public class SessionRequest
{
    public string hospitalType;
    public string mode;
    public string interactionMode;
}
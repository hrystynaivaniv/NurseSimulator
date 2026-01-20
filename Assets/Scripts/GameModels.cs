using System;
using System.Collections.Generic;

public enum CharacterRole { Any, Patient, Doctor, Nurse, Visitor }

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
    public List<VisitorData> visitors; 
    public List<InventoryItem> inventory; 
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
    public int totalScore; 
    public bool isGameOver;
    public int stepCount; 
    public int maxSteps;  
}

[Serializable]
public class SessionRequest
{
    public string hospitalType;
    public string mode;
    public string interactionMode;
}
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Person
{
    public string personName;
    public GameObject standingPrefab;
    public GameObject sittingPrefab;
    public GameObject lyingPrefab;
}

[System.Serializable]
public class SceneData
{
    public bool hasDoctor;
    public string patientGender; 
    public string[] visitors;
}

public class SceneManager : MonoBehaviour
{
    [Header("Character Prefabs")]
    public List<Person> maleCharacters;
    public List<Person> femaleCharacters;

    [Header("Medical Staff")]
    public GameObject doctorPrefab;

    [Header("Spawn Points")]
    public List<Transform> spawnPoints;

    private List<string> busyPeopleNames = new List<string>();
    public void SpawnSceneFromPython(string jsonData)
    {
        ClearScene();
        busyPeopleNames.Clear();

        SceneData data = JsonUtility.FromJson<SceneData>(jsonData);

        if (data.hasDoctor)
        {
            Transform docPoint = spawnPoints.FirstOrDefault(p => p.GetComponent<SpawnPointInfo>()?.assignedRole == SpawnPointInfo.PointRole.Doctor);
            if (docPoint != null) Spawn(doctorPrefab, docPoint, "Doctor");
        }

        Transform patientPoint = spawnPoints.FirstOrDefault(p => p.GetComponent<SpawnPointInfo>()?.assignedRole == SpawnPointInfo.PointRole.Patient);
        if (patientPoint != null)
        {
            List<Person> genderList = (data.patientGender == "Male") ? maleCharacters : femaleCharacters;
            var possiblePatients = genderList.Where(p => p.lyingPrefab != null).ToList();

            if (possiblePatients.Count > 0)
            {
                Person selected = possiblePatients[Random.Range(0, possiblePatients.Count)];
                Spawn(selected.lyingPrefab, patientPoint, "Patient");
                busyPeopleNames.Add(selected.personName);
            }
            else
            {
                Debug.LogWarning("Lying prefab not found for gender: " + data.patientGender);
            }
        }

        List<Transform> freePoints = spawnPoints
            .Where(p => p.GetComponent<SpawnPointInfo>()?.assignedRole == SpawnPointInfo.PointRole.Any)
            .OrderBy(x => Random.value).ToList();

        for (int i = 0; i < data.visitors.Length && i < freePoints.Count; i++)
        {
            string visitorRole = data.visitors[i];
            string gender = (visitorRole == "Woman") ? "Female" : "Male";

            Transform point = freePoints[i];
            bool isSitting = point.GetComponent<SpawnPointInfo>().isSittingPoint;

            Person selected = GetFreePerson(gender);
            if (selected != null)
            {
                GameObject prefabToSpawn = isSitting ? (selected.sittingPrefab ?? selected.standingPrefab) : selected.standingPrefab;

                Spawn(prefabToSpawn, point, visitorRole);
                busyPeopleNames.Add(selected.personName);
            }
        }
    }

    private Person GetFreePerson(string gender)
    {
        List<Person> list = (gender == "Male") ? maleCharacters : femaleCharacters;
        return list.FirstOrDefault(p => !busyPeopleNames.Contains(p.personName));
    }

    private GameObject Spawn(GameObject prefab, Transform point, string roleName)
    {
        if (prefab == null) return null;
        GameObject go = Instantiate(prefab, point.position, point.rotation);
        go.name = roleName;

        if (roleName == "Patient") go.tag = "Patient";

        return go;
    }

    private void ClearScene()
    {
        string[] targets = { "Doctor", "Patient", "Mother", "Male", "Visitor", "Woman" };

        var allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (var obj in allObjects)
        {
            if (targets.Contains(obj.name) || obj.CompareTag("Patient"))
            {
                Destroy(obj);
            }
        }
    }

    [ContextMenu("Test Scene Spawn")]
    public void TestSpawn()
    {
        string testJson = "{\"hasDoctor\":true,\"patientGender\":\"Female\",\"visitors\":[\"Female\",\"Male\"]}";
        SpawnSceneFromPython(testJson);
    }
}
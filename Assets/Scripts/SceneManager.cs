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

public class SceneManager : MonoBehaviour
{
    [Header("Character Prefabs")]
    public List<Person> maleCharacters;
    public List<Person> femaleCharacters;

    [Header("Medical Staff")]
    public GameObject doctorPrefab;

    [Header("Spawn Points")]
    public List<Transform> spawnPoints;

    [Header("Current State")]
    public List<InventoryItem> currentInventory;
    private List<GameObject> activeCharacters = new List<GameObject>();
    private List<string> busyPeopleNames = new List<string>();

    public void SpawnSceneFromData(InitialSceneData data)
    {
        ClearCurrentScene();
        busyPeopleNames.Clear();
        currentInventory = data.inventory;

        if (data.hasDoctor && doctorPrefab != null)
        {
            Transform docPoint = spawnPoints.FirstOrDefault(p => p.GetComponent<SpawnPointInfo>()?.assignedRole == SpawnPointInfo.PointRole.Doctor);
            if (docPoint != null)
            {
                Spawn(doctorPrefab, docPoint, "Doctor");
            }
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
        }

        if (data.visitors != null)
        {
            List<Transform> freePoints = spawnPoints
                .Where(p => p.GetComponent<SpawnPointInfo>()?.assignedRole == SpawnPointInfo.PointRole.Any)
                .OrderBy(x => Random.value).ToList();

            for (int i = 0; i < data.visitors.Count && i < freePoints.Count; i++)
            {
                var visitorData = data.visitors[i];
                string gender = visitorData.gender;

                Transform point = freePoints[i];
                var pointInfo = point.GetComponent<SpawnPointInfo>();
                bool isSitting = pointInfo != null && pointInfo.isSittingPoint;

                Person selected = GetFreePerson(gender);
                if (selected != null)
                {
                    GameObject prefabToSpawn = isSitting ? (selected.sittingPrefab ?? selected.standingPrefab) : selected.standingPrefab;
                    Spawn(prefabToSpawn, point, visitorData.role);
                    busyPeopleNames.Add(selected.personName);
                }
            }
        }
    }

    private Person GetFreePerson(string gender)
    {
        List<Person> list = (gender == "Male") ? maleCharacters : femaleCharacters;
        return list.FirstOrDefault(p => !busyPeopleNames.Contains(p.personName));
    }

    private void Spawn(GameObject prefab, Transform point, string roleName)
    {
        if (prefab == null) return;
        GameObject go = Instantiate(prefab, point.position, point.rotation);
        go.name = roleName;

        if (roleName == "Patient") go.tag = "Patient";

        activeCharacters.Add(go);
    }

    private void ClearCurrentScene()
    {
        foreach (GameObject charObj in activeCharacters)
        {
            if (charObj != null) Destroy(charObj);
        }
        activeCharacters.Clear();
    }

    public int GetItemCount(string itemName)
    {
        if (currentInventory == null) return 0;
        foreach (var item in currentInventory)
        {
            if (item.name == itemName) return item.count;
        }
        return 0;
    }
}
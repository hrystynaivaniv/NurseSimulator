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

    private GameLogicController _logic = new GameLogicController();
    private List<GameObject> activeCharacters = new List<GameObject>();

    public NetworkManager networkManager;

    private void OnEnable()
    {
        networkManager.OnSessionStarted += SpawnSceneFromData;
    }

    private void OnDisable()
    {
        networkManager.OnSessionStarted -= SpawnSceneFromData;
    }

    public void SpawnSceneFromData(InitialSceneData data)
    {
        ClearCurrentScene();

        var plan = _logic.PrepareFullScene(data);
        currentInventory = plan.StartingInventory;

        foreach (var instr in plan.Instructions)
        {
            Transform point = spawnPoints.FirstOrDefault(p =>
                (CharacterRole)p.GetComponent<SpawnPointInfo>().assignedRole == instr.TargetRole &&
                !IsPointOccupied(p));

            if (point == null) continue;

            GameObject prefabToSpawn = ResolvePrefab(instr, point.GetComponent<SpawnPointInfo>().isSittingPoint);
            Spawn(prefabToSpawn, point, instr.RoleName);
        }
    }

    private bool IsPointOccupied(Transform point) => activeCharacters.Any(c => Vector3.Distance(c.transform.position, point.position) < 0.1f);

    private GameObject ResolvePrefab(GameLogicController.SpawnInstruction instr, bool isSitting)
    {
        if (instr.IsDoctor) return doctorPrefab;

        var list = (instr.Gender == "Male") ? maleCharacters : femaleCharacters;
        var person = list.FirstOrDefault(p => !activeCharacters.Any(ac => ac.name == p.personName));

        if (person == null) return null;

        if (instr.ForceLying) return person.lyingPrefab;
        return isSitting ? person.sittingPrefab : person.standingPrefab;
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
        foreach (GameObject charObj in activeCharacters) if (charObj != null) Destroy(charObj);
        activeCharacters.Clear();
    }
}
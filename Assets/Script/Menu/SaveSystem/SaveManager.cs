using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour {

    public static SaveManager instance;
    public int selectedSaveSlot { get; set; }

    private PlanetarySystem _planetary;
    private LoadMapFactions _mapFactions;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }
    private void Start()
    {
        _planetary = FindAnyObjectByType<PlanetarySystem>();
        _mapFactions = FindAnyObjectByType<LoadMapFactions>();
    }
    public void SaveGameData()
    {
        if (_planetary == null) return;

        string nameData = Application.persistentDataPath + "/game_" + selectedSaveSlot + ".json";

        MapData data = new MapData
        {
            dataMinerals = _planetary.dataMinerals,
            dataPlanets = _planetary.dataPlanets
        };
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(nameData, json);
    }
    public void LoadGameData()
    {
        if(_mapFactions == null) return;

        string nameData = Application.persistentDataPath + "/game_" + selectedSaveSlot + ".json";

        string json = File.ReadAllText(nameData);
        MapData data = JsonUtility.FromJson<MapData>(json);

        _mapFactions._dataPlanets = data.dataPlanets;
        _mapFactions._dataMinerals = data.dataMinerals;
    }
}
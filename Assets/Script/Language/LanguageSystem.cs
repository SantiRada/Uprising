using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Language { Spanish = 0, English = 1, French = 2, Italian = 3 }
public class LanguageSystem : MonoBehaviour {

    [Header("Data CSV")]
    [SerializeField] private TextAsset _menuFile;
    [SerializeField] private TextAsset _gameFile;
    [SerializeField] private char _delimiter = '/';

    private static string[,] _menuData, _gameData;
    public static Language language = Language.Spanish;
    public static event Action changeLanguage;

    [Header("Data Result")]
    private TextMeshProUGUI[] _allText;
    private Text[] _allLabel;

    private void Awake()
    {
        LoadCSV();
        LoadAllText();
        UpdateLanguage();
    }
    private void Start()
    {
        changeLanguage += UpdateLanguage;
    }
    private void OnDestroy()
    {
        changeLanguage -= UpdateLanguage;
    }
    private void LoadCSV()
    {
        _gameData = ParseCSV(_gameFile);
        _menuData = ParseCSV(_menuFile);
    }
    private string[,] ParseCSV(TextAsset file)
    {
        string[] lines = file.text.Split('\n');
        int columnsExpected = lines[0].Split(_delimiter).Length - 1; // Excluir la primera columna (ID)

        string[,] data = new string[lines.Length - 1, columnsExpected]; // Excluir la primera fila (encabezados)

        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(_delimiter);

            for (int j = 1; j < columns.Length && (j - 1) < columnsExpected; j++)
            {
                data[i - 1, j - 1] = columns[j].Trim(); // Omitir la primera columna
            }
        }

        return data;
    }
    private void LoadAllText()
    {
        _allText = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        _allLabel = FindObjectsByType<Text>(FindObjectsSortMode.None);
    }
    private void UpdateLanguage()
    {
        foreach (var textElement in _allText)
        {
            string value = ExtractTextFromName(textElement.name);
            if (value != null) textElement.text = value;
        }

        foreach (var labelElement in _allLabel)
        {
            string value = ExtractTextFromName(labelElement.name);
            if (value != null) labelElement.text = value;
        }
    }
    private string ExtractTextFromName(string name)
    {
        if (!name.Contains("[") || !name.Contains("]") || !name.Contains("Text")) return null;

        string[] parts = name.Split(new char[] { '[', ']' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2) return null;

        string[] data = parts[1].Split(',');
        if (data.Length < 2) return null;

        return GetValue(data[0], int.Parse(data[1]));
    }
    public static string GetValue(string list, int rowIndex)
    {
        if (rowIndex <= 0) return "___INVALID INDEX___";
        list = list.ToLower();

        if (_gameData.GetLength(1) <= (int)language) return "_ NO SE ENCUENTRA: " + language + " en GAME_";
        if (_menuData.GetLength(1) <= (int)language) return "_ NO SE ENCUENTRA: " + language + " en MENU_";

        return list == "menu" && rowIndex <= _menuData.GetLength(0)
            ? _menuData[rowIndex - 1, (int)language]
            : _gameData[rowIndex - 1, (int)language];
    }
    public static void ChangeLanguage(int lang)
    {
        switch (lang)
        {
            case 0: language = Language.Spanish; break;
            case 1: language = Language.English; break;
            case 2: language = Language.French; break;
            case 3: language = Language.Italian; break;
        }

        changeLanguage?.Invoke();
    }
    public static int GetCountLanguage(string file)
    {
        if (file == "menu") return _menuData.GetLength(1);
        else return _gameData.GetLength(1);
    }
}
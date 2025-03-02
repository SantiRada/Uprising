using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlotGames : MonoBehaviour {

    public TextMeshProUGUI[] textSlots;
    private SaveManager _saveManager;

    private void Start()
    {
        _saveManager = FindAnyObjectByType<SaveManager>();

        LoadingScreen.finishLoading += InitialValues;
    }
    private void OnDestroy()
    {
        LoadingScreen.finishLoading -= InitialValues;
    }
    private void InitialValues()
    {
        for (int i = 0; i < textSlots.Length; i++)
        {
            string nameFile = Application.persistentDataPath + "/game_" + i + ".json";

            if (File.Exists(nameFile)) textSlots[i].text = LanguageSystem.GetValue("menu", 34) + "\n30%";
            else textSlots[i].text = LanguageSystem.GetValue("menu", 60);
        }
    }
    public void ReviewSlot(int num)
    {
        string nameFile = Application.persistentDataPath + "/game_" + num + ".json";

        _saveManager.selectedSaveSlot = num;

        if (File.Exists(nameFile))
        {
            // Cargar partida
            SceneManager.LoadScene("Game");
        }
        else
        {
            // Crear nueva partida
            SceneManager.LoadScene("PlanetarySystem");
        }
    }
}
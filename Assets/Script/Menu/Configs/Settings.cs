using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour {

    [Header("UI Content")]
    public Slider languageSlider;
    public Slider hudSlider;
    public Slider fovSlider;
    public Slider bgSubtitleSlider;
    public Slider sizeSubtitleSlider;
    [Space]
    public Slider fpsLimitSlider;
    public Toggle vsyncToggle;
    public Slider resolutionSlider;
    public Toggle fullscreenToggle;
    [Space]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    [Space]
    public Slider sensibilityMouseSlider;
    public Slider sensibilityStickSlider;
    public Toggle invertToggle;
    public Toggle vibrationToggle;

    [Header("Direction")]
    public string savePath;
    public static int volumenMaster = 7;
    public static int volumenMusic = 6;
    public static int volumenSFX = 8;
    public static bool vibrationGamepad = true;
    public event Action ChangeConfigs;

    private GameManager _manager;
    private Camera _cam;
    private Player _player;
    private ResolutionComponent _resolution;
    private ManagerHUD _dropdownHUD;
    private InputInterfaceSystem _inputs;

    private void Awake()
    {
        _cam = FindAnyObjectByType<Camera>();
        _manager = GetComponent<GameManager>();
        _player = FindAnyObjectByType<Player>();
        _dropdownHUD = FindAnyObjectByType<ManagerHUD>();
        _resolution = FindAnyObjectByType<ResolutionComponent>();

        savePath = Application.persistentDataPath + "/settings_save.json";
    }
    private void Start()
    {
        LoadGame();
    }
    public void SaveGame()
    {
        SettingsJSON data = GetConfigs();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        ChangeConfigs?.Invoke();

        LoadGame();
    }
    private SettingsJSON GetConfigs()
    {
        SettingsJSON data = new SettingsJSON
        {
            language = (int)languageSlider.value,
            visibilityHUD = (int)hudSlider.value,
            fov = (int)fovSlider.value,
            bgSubtitle = (int)bgSubtitleSlider.value,
            sizeSubtitle = (int)sizeSubtitleSlider.value,
            fpsLimit = (int)fpsLimitSlider.value,
            vsync = vsyncToggle.isOn,
            resolution = (int)resolutionSlider.value,
            fullscreen = fullscreenToggle.isOn,
            masterVolume = (int)masterSlider.value,
            musicVolume = (int)musicSlider.value,
            sfxVolume = (int)sfxSlider.value,
            sensibilityMouse = sensibilityMouseSlider.value,
            sensibilityStick = sensibilityStickSlider.value,
            invertY = invertToggle.isOn,
            vibration = vibrationToggle.isOn
        };

        return data;
    }
    public void LoadGame(int value = 0)
    {
        string completePath = savePath;
        if (value == -1 || !(File.Exists(savePath))) completePath = Application.persistentDataPath + "/base_configs.json";

        string json = File.ReadAllText(completePath);
        SettingsJSON data = JsonUtility.FromJson<SettingsJSON>(json);

        languageSlider.value = data.language;
        hudSlider.value = data.visibilityHUD;
        fovSlider.value = data.fov;
        bgSubtitleSlider.value = data.bgSubtitle;
        sizeSubtitleSlider.value = data.sizeSubtitle;

        fpsLimitSlider.value = data.fpsLimit;
        vsyncToggle.isOn = data.vsync;

        _resolution.position = (int)data.resolution;
        
        fullscreenToggle.isOn = data.fullscreen;

        masterSlider.value = data.masterVolume;
        musicSlider.value = data.musicVolume;
        sfxSlider.value = data.sfxVolume;

        sensibilityMouseSlider.value = data.sensibilityMouse;
        sensibilityStickSlider.value = data.sensibilityStick;
        invertToggle.isOn = data.invertY;
        vibrationToggle.isOn = data.vibration;

        StartCoroutine("ApplyVisuals");

        ApplyConfig(data);
    }
    private IEnumerator ApplyVisuals()
    {
        yield return new WaitForSeconds(0.4f);

        languageSlider.GetComponent<SliderWithOptions>().SetValues();

        hudSlider.GetComponent<SliderWithOptions>().SetValues();
        
        fpsLimitSlider.GetComponent<SliderWithOptions>().SetValues();

        _resolution.SetValues();
    }
    private void ApplyConfig(SettingsJSON data)
    {
        LanguageSystem.ChangeLanguage(data.language);
        _dropdownHUD.ChangeState(data.visibilityHUD);
        _cam.fieldOfView = fovSlider.value; // FOV

        QualitySettings.vSyncCount = vsyncToggle ? 1 : 0; // VSYNC 
        _resolution.SetResolutionExtern((int)resolutionSlider.value, data.fullscreen);

        volumenMaster = data.masterVolume;
        volumenMusic = data.musicVolume;
        volumenSFX = data.sfxVolume;

        _player.mouseSensitivity = data.sensibilityMouse;
        _player.stickSensitivity = data.sensibilityStick;
        _player.invertY = data.invertY;

        ChangeConfigs?.Invoke();
    }
    public void SendValue(int num, string parent)
    {
        parent = parent.ToLower().Trim();

        switch (parent)
        {
            case "lang":
                LanguageSystem.ChangeLanguage(num);
                LoadingScreen.RestartLoading();
                break;
            case "hud":
                _dropdownHUD.ChangeState(num);
                break;
            case "fps":
                _manager.ApplyLimitFPS(num);
                break;
            default: Debug.Log("No se reconoció el Input..."); break;
        }
    }
}
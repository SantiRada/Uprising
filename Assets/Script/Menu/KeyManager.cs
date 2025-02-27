using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class KeyManager : MonoBehaviour
{
    public Sprite[] sprXbox;
    public Sprite[] sprPlayStation;
    public string[] nameSprites;

    private string _device;
    private PlayerInput _input;
    private InputInterfaceSystem _inputs;

    private List<TextMeshProUGUI> _listText = new List<TextMeshProUGUI>();
    private Dictionary<string, int> _spriteIndexMap = new Dictionary<string, int>();
    private static KeyManager instance;

    private void Awake()
    {
        _input = FindAnyObjectByType<PlayerInput>();
        _inputs = FindAnyObjectByType<InputInterfaceSystem>();

        foreach (var text in FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None))
        {
            if (text.name.ToLower().Contains("key"))
                _listText.Add(text);
        }

        for (int i = 0; i < nameSprites.Length; i++)
            _spriteIndexMap[nameSprites[i].ToLower()] = i;
    }
    private void Start()
    {
        instance = this;

        _inputs.changeScheme += LoadKeys;
        LoadKeys();
    }
    private void OnDestroy()
    {
        _inputs.changeScheme -= LoadKeys;
    }
    private void LoadKeys()
    {
        DetectDevice();

        foreach (var text in _listText)
        {
            string[] textKey = ExtractTextFromName(text.name);
            if (textKey == null) continue;

            string actionMapName = textKey[0].ToLower();
            string bindingName = textKey[1];

            InputActionMap actionMap = _input.actions.FindActionMap(actionMapName, false);
            InputAction action = actionMap?.FindAction(bindingName, false);
            if (action == null) continue;

            InputBinding? binding = action.bindings.FirstOrDefault(b => b.groups.Contains(_input.currentControlScheme));
            if (!binding.HasValue) continue;

            string actionBinding = InputControlPath.ToHumanReadableString(binding.Value.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

            Image img = text.transform.parent.GetComponent<Image>();

            if (_device == "keyboard")
            {
                switch (actionBinding.ToLower())
                {
                    case "backspace": actionBinding = "←"; break;
                    case "return": actionBinding = LanguageSystem.GetValue("menu", 55); break;
                    case "escape": actionBinding = "ESC"; break;

                    case "up arrow": actionBinding = "↑"; break;
                    case "down arrow": actionBinding = "↓"; break;
                    case "left arrow": actionBinding = "←"; break;
                    case "right arrow": actionBinding = "→"; break;

                    case "mouse left": actionBinding = LanguageSystem.GetValue("menu", 53); break;
                    case "mouse right": actionBinding = LanguageSystem.GetValue("menu", 54); break;
                    case "mouse middle": actionBinding = LanguageSystem.GetValue("menu", 52); break;
                }

                text.text = actionBinding;
                img.sprite = null;
            }
            else
            {
                text.text = "";
                img.sprite = GetSprite(actionBinding);
            }
        }
    }
    private void DetectDevice()
    {
        _device = "keyboard";
        string currentScheme = _input.currentControlScheme.ToLower().Trim();

        if (currentScheme.Contains("gamepad") && Input.GetJoystickNames().Length > 0)
        {
            string joystickName = Input.GetJoystickNames()[0]?.ToLower().Trim();
            _device = joystickName.Contains("xbox") ? "xbox" : "play station";
        }
    }
    private Sprite GetSprite(string action)
    {
        action = action.ToLower().Trim();

        if (_spriteIndexMap.TryGetValue(action, out int index)) return _device == "xbox" ? sprXbox[index] : sprPlayStation[index];

        return null;
    }
    private string[] ExtractTextFromName(string name)
    {
        if (!name.Contains("[") || !name.Contains("]") || !name.ToLower().Contains("key"))
            return null;

        string[] parts = name.Split(new char[] { '[', ']' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2) return null;

        string[] data = parts[1].Split(',');
        return data;
    }
    public static string GetNameKey(string name)
    {
        InputAction action = instance._input.actions.FindAction(name);
        if (action == null)
        {
            Debug.LogWarning($"No se encontró la acción '{name}'");
            return null;
        }

        int bindingIndex = action.GetBindingIndexForControl(action.controls[0]);
        if (bindingIndex == -1)
        {
            Debug.LogWarning($"No se encontró un binding activo para '{name}'");
            return null;
        }

        return action.GetBindingDisplayString(bindingIndex);
    }
}
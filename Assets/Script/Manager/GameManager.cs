using TMPro;
using UnityEngine;

public enum FPSLimit { Thirty = 30, Sixty = 60, Ninety = 90, OneTwenty = 120, Unlimited = 0 }
public class GameManager : MonoBehaviour {

    public FPSLimit targetFPS = FPSLimit.Sixty;
    public TextMeshProUGUI fpsText;

    private float deltaTime = 0.0f;

    private void Start()
    {
        ApplyLimitFPS(-1);
    }
    public void ApplyLimitFPS(int num)
    {
        switch (num)
        {
            case 0:     targetFPS = FPSLimit.Thirty;       break;
            case 1:     targetFPS = FPSLimit.Sixty;        break;
            case 2:     targetFPS = FPSLimit.Ninety;       break;
            case 3:     targetFPS = FPSLimit.OneTwenty;    break;
            case 4:     targetFPS = FPSLimit.Unlimited;    break;
            default:    targetFPS = FPSLimit.Sixty;        break;
        }

        Application.targetFrameRate = (int)targetFPS;
    }
    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";
    }
}

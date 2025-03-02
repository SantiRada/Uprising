using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    public GameObject pauseUI;

    private InputPlayerSystem _inputs;
    private static PauseMenu instance;
    private MenuManager _menu;

    private void Awake()
    {
        instance = this;
        _inputs = FindAnyObjectByType<InputPlayerSystem>();
        _menu = FindAnyObjectByType<MenuManager>();
    }
    private void Start()
    {
        LoadingScreen.finishLoading += InitialValues;

        _inputs.useEscape += ReviewPause;
    }
    private void InitialValues()
    {
        MenuController.state = StatePlayer.Game;
        pauseUI.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        LoadingScreen.finishLoading -= InitialValues;

        _inputs.useEscape -= ReviewPause;
    }
    private void ReviewPause()
    {
        if (MenuController.state == StatePlayer.UserInterface) return;

        if (MenuController.state == StatePlayer.Game) Pause();
        else if (MenuController.state == StatePlayer.Pause) Continue();
    }
    public void Continue()
    {
        pauseUI.gameObject.SetActive(false);
        MenuController.state = StatePlayer.Game;
    }
    public void Pause()
    {
        _menu.OpenSector(0);

        pauseUI.gameObject.SetActive(true);
        MenuController.state = StatePlayer.Pause;
        
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public static void SetState(StatePlayer newState)
    {
        if (MenuController.state == StatePlayer.UserInterface && instance != null && newState == StatePlayer.Game)
        {
            instance.StartCoroutine(instance.SetStateWithDelay(newState));
        }
        else
        {
            MenuController.state = newState;
        }
    }
    private IEnumerator SetStateWithDelay(StatePlayer newState)
    {
        yield return new WaitForSeconds(0.18f);
        MenuController.state = newState;

        if(newState == StatePlayer.Game)
        {
            pauseUI.gameObject.SetActive(false);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
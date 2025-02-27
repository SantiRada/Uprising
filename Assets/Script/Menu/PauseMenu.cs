using UnityEngine;
using System.Collections;

public enum StatePlayer { UserInterface, Game, Pause };

public class PauseMenu : MonoBehaviour {

    public GameObject pauseUI;

    public static StatePlayer state;
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
        state = StatePlayer.Game;
        pauseUI.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        LoadingScreen.finishLoading -= InitialValues;

        _inputs.useEscape -= ReviewPause;
    }
    private void ReviewPause()
    {
        if (state == StatePlayer.UserInterface) return;

        if (state == StatePlayer.Game) Pause();
        else if (state == StatePlayer.Pause) Continue();
    }
    public void Continue()
    {
        pauseUI.gameObject.SetActive(false);
        state = StatePlayer.Game;
    }
    public void Pause()
    {
        _menu.OpenSector(0);

        pauseUI.gameObject.SetActive(true);
        state = StatePlayer.Pause;
        
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public static void SetState(StatePlayer newState)
    {
        if (state == StatePlayer.UserInterface && instance != null && newState == StatePlayer.Game)
        {
            instance.StartCoroutine(instance.SetStateWithDelay(newState));
        }
        else
        {
            state = newState;
        }
    }
    private IEnumerator SetStateWithDelay(StatePlayer newState)
    {
        yield return new WaitForSeconds(0.18f);
        state = newState;

        if(newState == StatePlayer.Game)
        {
            pauseUI.gameObject.SetActive(false);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
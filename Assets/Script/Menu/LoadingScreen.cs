using NUnit.Framework.Constraints;
using System;
using System.Collections;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {

    public float timeToLoading;
    public GameObject loadingScreen;

    public static event Action finishLoading;
    public static event Action isLoading;

    private InputInterfaceSystem _inputs;

    private void Awake()
    {
        _inputs = FindAnyObjectByType<InputInterfaceSystem>();
    }
    private void Start()
    {
        isLoading += () => StartCoroutine("Loading");

        StartCoroutine("Loading");
    }
    private void OnDestroy()
    {
        isLoading -= () => StartCoroutine("Loading");
    }
    private IEnumerator Loading()
    {
        loadingScreen.gameObject.SetActive(true);

        yield return new WaitForSeconds(timeToLoading / 2);

        finishLoading?.Invoke();

        yield return new WaitForSeconds(timeToLoading / 2);

        loadingScreen.gameObject.SetActive(false);

        PauseMenu.SetState(StatePlayer.Game);

        _inputs.ChangeActionMap(false);
    }
    public static void RestartLoading() { isLoading?.Invoke(); }
}
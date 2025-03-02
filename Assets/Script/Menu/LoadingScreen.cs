using NUnit.Framework.Constraints;
using System;
using System.Collections;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {

    public float timeToLoading;
    public GameObject loadingScreen;

    public static event Action finishLoading;
    public static event Action isLoading;

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
        MenuController.state = StatePlayer.Pause;

        loadingScreen.gameObject.SetActive(true);

        yield return new WaitForSeconds(timeToLoading / 2);

        finishLoading?.Invoke();

        yield return new WaitForSeconds(timeToLoading / 2);

        loadingScreen.gameObject.SetActive(false);

        MenuController.state = StatePlayer.Game;
    }
    public static void RestartLoading() { isLoading?.Invoke(); }
}
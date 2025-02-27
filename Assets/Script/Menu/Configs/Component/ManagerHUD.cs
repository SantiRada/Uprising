using System.Collections.Generic;
using UnityEngine;

public enum StateHUD { visible, soloEnUso, invisible }
public class ManagerHUD : MonoBehaviour {

    public List<GameObject> allUI = new List<GameObject>();
    public int jetpackPos;
    [Space]
    public StateHUD state;
    public float delayToHide;

    private float _baseDelay;
    private InputPlayerSystem _inputs;

    private void Awake()
    {
        _inputs = FindAnyObjectByType<InputPlayerSystem>();
    }
    private void Start()
    {
        _baseDelay = delayToHide;
    }
    private void Update()
    {
        if (state != StateHUD.soloEnUso) return;

        if (delayToHide > 0) delayToHide -= Time.deltaTime;
        else HideUI();
    }
    public void ChangeState(int pos)
    {
        switch (pos)
        {
            case 0: state = StateHUD.visible; break;
            case 1: state = StateHUD.soloEnUso; break;
            case 2: state = StateHUD.invisible; break;
        }

        CheckState();
    }
    private void CheckState()
    {
        if (state == StateHUD.visible)
        {
            for (int i = 0; i < allUI.Count; i++)
            {
                allUI[i].gameObject.SetActive(true);
            }
        }
        else if (state == StateHUD.soloEnUso)
        {
            _inputs.useJetpackEvent += ShowUI;
        }
        else
        {
            for (int i = 0; i < allUI.Count; i++)
            {
                allUI[i].gameObject.SetActive(false);
            }
        }
    }
    private void OnDestroy()
    {
        if (state == StateHUD.soloEnUso)
            _inputs.useJetpackEvent -= ShowUI;
    }
    private void HideUI()
    {
        allUI[jetpackPos].gameObject.SetActive(false);
    }
    private void ShowUI()
    {
        allUI[jetpackPos].gameObject.SetActive(true);
    }
}
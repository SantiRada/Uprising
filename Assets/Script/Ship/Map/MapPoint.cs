using System.Collections;
using TMPro;
using UnityEngine;

public class MapPoint : MonoBehaviour {

    [Header("Content UI")]
    public GameObject factionUI;
    public TextMeshProUGUI titleFaction;
    public float offsetY;
    private bool _isOpen = false;

    [Header("Info Move")]
    public float detectionRange = 50f;
    
    [Header("Faction Data")]
    public int nameFaction;
    public GameObject[] factionObj;
    public GameObject[] planetObj;
    public GameObject parent;

    private MapPlayer _player;
    private RectTransform _thisTransform;
    private RectTransform _playerTransform;
    private InputInterfaceSystem _inputs;

    private void Awake()
    {
        _thisTransform = GetComponent<RectTransform>();
        _player = FindAnyObjectByType<MapPlayer>();
        _playerTransform = _player.GetComponent<RectTransform>();

        _inputs = FindAnyObjectByType<InputInterfaceSystem>();
    }
    private void Start()
    {
        LoadingScreen.finishLoading += InitialValues;
        _inputs.useSelect += GoToFaction;
    }
    private void InitialValues()
    {
        for (int i = 0; i < factionObj.Length; i++)
        {
            factionObj[i].gameObject.SetActive(false);
        }

        _isOpen = false;
        factionUI.SetActive(false);
    }
    private void OnDestroy()
    {
        LoadingScreen.finishLoading -= InitialValues;

        _inputs.useSelect -= GoToFaction;
    }
    private void Update()
    {
        float distance = Vector2.Distance(_playerTransform.anchoredPosition, _thisTransform.anchoredPosition);

         if (distance <= detectionRange) OpenUI();

        if (_isOpen && distance > detectionRange) CloseUI();
    }
    private void OpenUI()
    {
        _isOpen = true;

        factionUI.SetActive(true);

        titleFaction.text = LanguageSystem.GetValue("menu", nameFaction);

        Vector3 newPosition = new Vector3(_thisTransform.position.x, _thisTransform.position.y + offsetY, _thisTransform.position.z);
        factionUI.GetComponent<RectTransform>().SetPositionAndRotation(newPosition, Quaternion.identity);
    }
    private void CloseUI()
    {
        _isOpen = false;

        factionUI.SetActive(false);
    }
    private void GoToFaction()
    {
        if (_isOpen) StartCoroutine("TravelToFaction");
    }
    private IEnumerator TravelToFaction()
    {
        StartCoroutine(ChangeDistance(1.75f));

        yield return new WaitForSeconds(2.5f);

        for(int i = 0; i < planetObj.Length; i++)
        {
            planetObj[i].gameObject.SetActive(false);
        }

        for(int i = 0; i < factionObj.Length; i++)
        {
            factionObj[i].gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(ChangeDistance(1f));
    }
    private IEnumerator ChangeDistance(float target)
    {
        Vector3 targetSize = new Vector3(target, target, target);

        Vector2 startSize = new Vector2(parent.transform.localScale.x, parent.transform.localScale.y);
        float elapsedTime = 0f;

        while (elapsedTime < 2f)
        {
            Vector3 newSize = Vector3.Lerp(startSize, targetSize, elapsedTime / 2f);
            parent.transform.localScale = newSize;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        parent.transform.localScale = targetSize;
    }
}

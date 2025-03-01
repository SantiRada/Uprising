using System.Collections;
using TMPro;
using UnityEngine;

public class MapPoint : MonoBehaviour {

    [Header("Content UI")]
    public float offsetY;
    public GameObject factionUI;
    public TextMeshProUGUI titleFaction;
    private bool _isOpen = false;
    private bool _isTraveling = false;

    [Header("Info Move")]
    public float detectionRange = 75f;
    
    [Header("Faction Data")]
    public int nameFaction;
    public GameObject planetObj;
    [HideInInspector] public GameObject factionObj;
    private GameObject parentObj;

    private MapPlayer _player;
    private RectTransform _thisTransform;
    private RectTransform _playerTransform;
    private InputInterfaceSystem _inputs;

    private void Awake()
    {
        _thisTransform = GetComponent<RectTransform>();
        _player = FindAnyObjectByType<MapPlayer>();
        _playerTransform = _player.GetComponent<RectTransform>();
        parentObj = transform.parent.gameObject;

        _inputs = FindAnyObjectByType<InputInterfaceSystem>();
    }
    private void Start()
    {
        #region ReconocimientoDeUI
        factionUI = GameObject.Find("FactionUI");
        TextMeshProUGUI[] texts = factionUI.GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].name == "TitleFaction")
            {
                titleFaction = texts[i];
                break;
            }
        }
        #endregion

        LoadingScreen.finishLoading += InitialValues;
        _inputs.useSelect += GoToFaction;
    }
    private void InitialValues()
    {
        factionObj.gameObject.SetActive(false);

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

         if (distance <= detectionRange && !_isOpen) OpenUI();
         if (_isOpen && distance > detectionRange) CloseUI();
    }
    private void OpenUI()
    {
        _isOpen = true;

        factionUI.SetActive(true);
        factionUI.GetComponent<RectTransform>().SetAsLastSibling();

        titleFaction.text = LanguageSystem.GetValue("rogue", nameFaction);

        Vector3 newPosition = new Vector3(_thisTransform.position.x, _thisTransform.position.y + offsetY, _thisTransform.position.z);
        factionUI.GetComponent<RectTransform>().SetPositionAndRotation(newPosition, Quaternion.identity);
    }
    private void CloseUI()
    {
        if (_isTraveling) return;

        _isOpen = false;

        factionUI.SetActive(false);
    }
    private void GoToFaction()
    {
        if (_isOpen) StartCoroutine("TravelToFaction");
    }
    private IEnumerator TravelToFaction()
    {
        _isTraveling = true;
        factionUI.SetActive(false);

        StartCoroutine(ChangeDistance(1.75f));

        yield return new WaitForSeconds(1.2f);

        planetObj.gameObject.SetActive(false);
        factionObj.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.2f);

        StartCoroutine(ChangeDistance(1f));
        _isOpen = false;
        _isTraveling = false;
    }
    private IEnumerator ChangeDistance(float target)
    {
        Vector3 targetSize = new Vector3(target, target, target);

        Vector2 startSize = new Vector2(parentObj.transform.localScale.x, parentObj.transform.localScale.y);
        float elapsedTime = 0f;

        while (elapsedTime < 1.5f)
        {
            Vector3 newSize = Vector3.Lerp(startSize, targetSize, elapsedTime / 1.5f);
            parentObj.transform.localScale = newSize;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        parentObj.transform.localScale = targetSize;
    }
}
using System.Collections;
using UnityEngine;

public class MapPoint : MonoBehaviour {

    [Header("Content UI")]
    public FactionUI factionUI;
    private bool _isOpen = false;
    private bool _isTraveling = false;

    [Header("Info Move")]
    public float detectionRange = 75f;
    
    private RectTransform _thisTransform;
    private RectTransform _playerTransform;
    private InputInterfaceSystem _inputs;

    [Header("Planet")]
    public int namePlanet;
    public Mineral receivedMineral;
    public Mineral giveMineral;

    private void OnEnable()
    {
        _thisTransform = GetComponent<RectTransform>();
        _playerTransform = FindAnyObjectByType<MapPlayer>().GetComponent<RectTransform>();

        _inputs = FindAnyObjectByType<InputInterfaceSystem>();
    }
    private void Start()
    {
        factionUI = FindAnyObjectByType<FactionUI>();

        LoadingScreen.finishLoading += InitialValues;
        _inputs.useSelect += GoToFaction;
    }
    private void InitialValues()
    {
        _isOpen = false;
        factionUI.gameObject.SetActive(false);
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
        StartCoroutine("OpenFactionUI");
    }
    private IEnumerator OpenFactionUI()
    {
        _isOpen = true;

        factionUI.gameObject.SetActive(true);

        yield return null;

        factionUI.OpenUI(namePlanet, receivedMineral, giveMineral, _thisTransform);
    }
    private void CloseUI()
    {
        if (_isTraveling) return;

        _isOpen = false;

        factionUI.gameObject.SetActive(false);
    }
    private void GoToFaction()
    {
        if (_isOpen) StartCoroutine("TravelToFaction");
    }
    private IEnumerator TravelToFaction()
    {
        _isTraveling = true;
        factionUI.gameObject.SetActive(false);

        StartCoroutine(ChangeDistance(1.75f));

        yield return new WaitForSeconds(1.2f);

        StartCoroutine(ChangeDistance(1f));
        _isOpen = false;
        _isTraveling = false;
    }
    private IEnumerator ChangeDistance(float target)
    {
        Vector3 targetSize = new Vector3(target, target, target);

        Vector2 startSize = new Vector2(transform.parent.transform.localScale.x, transform.parent.transform.localScale.y);
        float elapsedTime = 0f;

        while (elapsedTime < 1.5f)
        {
            Vector3 newSize = Vector3.Lerp(startSize, targetSize, elapsedTime / 1.5f);
            transform.parent.transform.localScale = newSize;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.parent.transform.localScale = targetSize;
    }
}
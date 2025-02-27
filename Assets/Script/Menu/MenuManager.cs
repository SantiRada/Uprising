using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Reset Functionality")]
    [SerializeField] private GameObject resetSector;
    private bool _inReset;

    [Header("Sectors")]
    [SerializeField] private Selectable[] firstSelectable;
    [SerializeField] private GameObject[] sectioners;
    private int _currentSectorIndex;

    [Header("Categories Config")]
    [SerializeField] private Color unselectedOption;
    [SerializeField] private Color selectedOption;
    [SerializeField] private Image[] categoryButtons;
    [SerializeField] private GameObject[] categoriesSectors;
    [SerializeField] private Selectable[] firstCategorySelect;
    [SerializeField] private int[] countForCategory;
    private int _currentCategoryIndex;
    private bool _isInCategories;

    [Header("Movement")]
    [SerializeField] private float delayToMove;
    private float _baseDelayToMove;
    private bool _canMove = true;

    [Header("Config Sector")]
    [SerializeField] private Transform selector;
    [SerializeField] private Selectable[] selectables;
    private int _moveInCategory;

    [Header("Control Sector")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    private int _lastScrollMove;

    private InputInterfaceSystem _inputs;
    private ModifyControls _modifier;
    private Settings _settings;

    private void Awake()
    {
        _modifier = FindAnyObjectByType<ModifyControls>();
        _inputs = GetComponent<InputInterfaceSystem>();
        _settings = GetComponent<Settings>();
    }
    private void OnEnable()
    {
        LoadingScreen.finishLoading += InitializeUI;

        if (_inputs != null) SubscribeToInputEvents();
    }
    private void OnDisable()
    {
        LoadingScreen.finishLoading -= InitializeUI;
        UnsubscribeFromInputEvents();
    }
    private void Start () => _baseDelayToMove = delayToMove;
    private void Update()
    {
        if (!_canMove) return;

        delayToMove -= Time.deltaTime;

        if (!_isInCategories) return;

        if (delayToMove <= 0 && !Mathf.Approximately(_inputs.movement.y, 0)) MoveInUI(_inputs.movement.y > 0 ? 1 : -1);
    }
    private void SubscribeToInputEvents()
    {
        _inputs.useBack += CloseSector;
        _inputs.usePrevious += MoveLeft;
        _inputs.useNext += MoveRight;
        _inputs.useReset += OpenReset;
        _inputs.useSelect += SelectReset;
    }
    private void UnsubscribeFromInputEvents()
    {
        if (_inputs != null)
        {
            _inputs.useBack -= CloseSector;
            _inputs.usePrevious -= MoveLeft;
            _inputs.useNext -= MoveRight;
            _inputs.useReset -= OpenReset;
            _inputs.useSelect -= SelectReset;
        }
    }
    private void InitializeUI()
    {
        resetSector.SetActive(false);

        // Reset category button colors
        for (int i = 0; i < categoryButtons.Length; i++)
        {
            categoryButtons[i].color = unselectedOption;
        }

        categoryButtons[0].color = selectedOption;

        _currentCategoryIndex = categoriesSectors.Length - 1;
        _canMove = true;

        // Deactivate all category sectors
        foreach (var sector in categoriesSectors)
        {
            sector.SetActive(false);
        }

        // Deactivate all sectioners
        foreach (var section in sectioners)
        {
            section.SetActive(false);
        }
    }
    private void SelectReset()
    {
        if (!_inReset) return;
        _settings.LoadGame(-1);
    }
    private void OpenReset()
    {
        if (_modifier.inControls) return;

        if (_inReset)
        {
            CloseReset();
            return;
        }

        resetSector.SetActive(true);
        _canMove = false;
        _inReset = true;
    }
    private void CloseReset()
    {
        _inReset = false;
        _canMove = true;
        resetSector.SetActive(false);
    }
    public void OpenSector(int sectorIndex)
    {
        if (_modifier.inControls) return;

        if (sectorIndex == 0)
        {
            OpenCategories();
            _inputs.ChangeActionMap(true);
            Invoke(nameof(SelectInitialSector), 0.2f);
        }
        else
        {
            firstSelectable[sectorIndex].Select();
        }

        _isInCategories = sectorIndex == 1;
        _currentSectorIndex = sectorIndex;
        sectioners[sectorIndex].SetActive(true);
    }
    private void SelectInitialSector() => firstSelectable[0].Select();
    public void CloseSector()
    {
        if (_modifier.inControls) return;

        if (_inReset)
        {
            CloseReset();
            return;
        }

        if (_currentSectorIndex > 0)
        {
            sectioners[_currentSectorIndex].SetActive(false);
            _currentSectorIndex--;
            OpenSector(_currentSectorIndex);
            _settings.SaveGame();
        }
        else
        {
            _inputs.ChangeActionMap(false);
            PauseMenu.SetState(StatePlayer.Game);
        }
    }
    private void MoveLeft()
    {
        if (CanNavigateCategories()) MoveInCategories(false);
    }
    private void MoveRight()
    {
        if (CanNavigateCategories()) MoveInCategories(true);
    }
    private bool CanNavigateCategories()
    {
        return _isInCategories && !_inReset && _canMove && !_modifier.inControls;
    }
    private void OpenCategories()
    {
        if (_modifier.inControls) return;

        foreach (var sector in categoriesSectors)
        {
            sector.SetActive(false);
        }

        _canMove = true;
        categoriesSectors[0].SetActive(true);
        _currentCategoryIndex = 0;
        _moveInCategory = 0;

        // Update category button colors
        UpdateCategoryButtonColors();

        selectables[_moveInCategory].Select();
        UpdateSelectorParent();
        MoveSelector();
    }
    public void MoveToCategory(int categoryIndex)
    {
        if (_modifier.inControls) return;

        categoryButtons[_currentCategoryIndex].color = unselectedOption;
        categoriesSectors[_currentCategoryIndex].SetActive(false);

        _currentCategoryIndex = categoryIndex;
        _moveInCategory = GetCurrentSize(_currentCategoryIndex);

        categoryButtons[_currentCategoryIndex].color = selectedOption;
        categoriesSectors[_currentCategoryIndex].SetActive(true);

        selectables[_moveInCategory].Select();
        UpdateSelectorParent();
        MoveSelector();
    }
    public void MoveInCategories(bool isRight)
    {
        if (_modifier.inControls) return;

        categoryButtons[_currentCategoryIndex].color = unselectedOption;
        categoriesSectors[_currentCategoryIndex].SetActive(false);
        _canMove = true;

        // Calculate new category index with wraparound
        _currentCategoryIndex = isRight ?
            (_currentCategoryIndex + 1) % categoriesSectors.Length :
            (_currentCategoryIndex - 1 + categoriesSectors.Length) % categoriesSectors.Length;

        _moveInCategory = GetCurrentSize(_currentCategoryIndex);

        categoryButtons[_currentCategoryIndex].color = selectedOption;
        categoriesSectors[_currentCategoryIndex].SetActive(true);

        selectables[_moveInCategory].Select();
        UpdateSelectorParent();
        MoveSelector();
    }
    private void UpdateSelectorParent()
    {
        selector.SetParent(_currentCategoryIndex != 3 ?
            categoriesSectors[_currentCategoryIndex].transform :
            content.transform);
    }
    private void UpdateCategoryButtonColors()
    {
        for (int i = 0; i < categoryButtons.Length; i++)
        {
            categoryButtons[i].color = unselectedOption;
        }
        categoryButtons[_currentCategoryIndex].color = selectedOption;
    }
    private void MoveSelector()
    {
        Vector3 newPosition = new Vector3(
            selector.position.x,
            selectables[_moveInCategory].transform.position.y,
            selector.position.z);

        selector.position = newPosition;
        selector.SetAsFirstSibling();

        AdjustScrollPosition();
    }
    private void AdjustScrollPosition()
    {
        if (!scrollRect || !content) return;

        int firstElement = GetCurrentSize(_currentCategoryIndex);
        int lastElement = firstElement + countForCategory[_currentCategoryIndex] - 1;

        if (Mathf.Abs(_moveInCategory - _lastScrollMove) < 2) return;

        float scrollValue = Mathf.InverseLerp(firstElement, lastElement, _moveInCategory);
        float targetPosition = Mathf.Clamp(1 - scrollValue, 0.15f, 0.95f);

        StartCoroutine(SmoothScroll(targetPosition));

        _lastScrollMove = _moveInCategory;
    }
    private IEnumerator SmoothScroll(float target)
    {
        float duration = 0.2f;
        float elapsedTime = 0;
        float startValue = scrollRect.verticalNormalizedPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(startValue, target, elapsedTime / duration);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = target;
    }
    private void MoveInUI(int direction)
    {
        direction = -direction;
        delayToMove = _baseDelayToMove;

        int baseIndex = GetCurrentSize(_currentCategoryIndex);
        int totalOptions = countForCategory[_currentCategoryIndex];
        int relativeIndex = _moveInCategory - baseIndex;

        // Calculate new index with wraparound
        relativeIndex = (relativeIndex + direction + totalOptions) % totalOptions;
        _moveInCategory = baseIndex + relativeIndex;

        selectables[_moveInCategory].Select();
        MoveSelector();
    }
    private int GetCurrentSize(int categoryIndex)
    {
        int size = 0;
        for (int i = 0; i < categoryIndex; i++)
        {
            size += countForCategory[i];
        }
        return size;
    }
}
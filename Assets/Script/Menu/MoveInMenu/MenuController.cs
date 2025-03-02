using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum StatePlayer { UserInterface, Game, Pause }
public class MenuController : MonoBehaviour {

    [Header("Sectors")]
    public bool firstInUI;
    public GameObject[] sectors;
    public Selectable[] firstSelectable;
    public bool[] hasCategory;
    public GameObject resetSector;
    private int indexSector = 0;
    private int prevIndexSector = 0;

    [Header("Selectables")]
    public RectTransform selector;
    public float diffSize = 24;
    [Range(0f, 0.1f)] public float timeDeath = 0.025f;
    // Para que se encuentren de forma dinámica su nombre debe comenzar por "S_"
    private List<Selectable> allSelectables = new List<Selectable>();

    [Header("Categories")]
    public Color unselectedCategory;
    public Color selectedCategory;
    // Para que se encuentren de forma dinámica su nombre debe comenzar por "B_"
    private List<Image> buttonCategories = new List<Image>();
    // Para que se encuentren de forma dinámica deben tener la palabra "Category_" en su nombre
    private List<RectTransform> categoriesToReview = new List<RectTransform>();
    private int indexCategory = 0;

    [Header("Private Content")]
    public static StatePlayer state { get; set; }
    private InputInterfaceSystem _inputs;

    private void Awake()
    {
        _inputs = FindAnyObjectByType<InputInterfaceSystem>();
    }
    private void Start()
    {
        LoadingScreen.finishLoading += InitialValues;

        _inputs.useMove += PrevMove;
        _inputs.useBack += CloseSector;
        _inputs.usePrevious += MoveLeft;
        _inputs.useNext += MoveRight;
    }
    private void OnDestroy()
    {
        LoadingScreen.finishLoading -= InitialValues;

        _inputs.useMove -= PrevMove;
        _inputs.useBack -= CloseSector;
        _inputs.usePrevious -= MoveLeft;
        _inputs.useNext -= MoveRight;
    }
    private void InitialValues()
    {
        foreach (var sector in sectors) sector.SetActive(false);

        if (firstInUI)
        {
            state = StatePlayer.UserInterface;
            _inputs.ChangeActionMap(true);
            OpenSector(0);
        }
        else
        {
            state = StatePlayer.Game;
            _inputs.ChangeActionMap(false);
        }

        resetSector.gameObject.SetActive(false);
    }
    ///////////////////////////////////////////////
    public void OpenSector(int sector)
    {
        if (sector < 0 || sector >= sectors.Length) return;

        if (indexSector != sector) sectors[indexSector].gameObject.SetActive(false);

        sectors[sector].gameObject.SetActive(true);
        firstSelectable[sector].Select();

        prevIndexSector = indexSector;
        indexSector = sector;
        indexCategory = 0;

        // Selector Manager
        DefineSelector(sectors[indexSector].transform);

        if (hasCategory[sector]) FindCategories();
    }
    public void CloseSector()
    {
        ResetCategories();

        if(prevIndexSector == indexSector || indexSector == 0) Debug.LogAssertion("Debe cerrar todo el menú.");
        else OpenSector(prevIndexSector);
    }
    private void PrevMove() => Invoke("MoveSelector", timeDeath);
    ///////////////////////////////////////////////
    private void DefineSelector(Transform obj)
    {
        selector.SetParent(obj, false);
        selector.SetAsFirstSibling();
        Invoke("MoveSelector", timeDeath);
    }
    private void MoveSelector()
    {
        RectTransform selected = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

        Vector3 newPosition = new Vector3(selector.position.x, selected.position.y, selector.position.z);
        Vector2 newSize = new Vector2(selector.parent.GetComponent<RectTransform>().sizeDelta.x, selected.sizeDelta.y + diffSize);

        selector.SetPositionAndRotation(newPosition, Quaternion.identity);
        selector.sizeDelta = newSize;
    }
    ///////////////////////////////////////////////
    private void MoveLeft() => MoveCategory(-1);
    private void MoveRight() => MoveCategory(1);
    private void MoveCategory(int dir)
    {
        if (!hasCategory[indexSector]) return;

        int num = indexCategory;

        if(dir > 0)
        {
            num++;

            if (num >= categoriesToReview.Count) num = 0;
        }
        else
        {
            num--;

            if(num < 0) num = (categoriesToReview.Count - 1);
        }

        SelectCategory(num);

    }
    public void ResetCategories()
    {
        for (int i = 0; i < categoriesToReview.Count; i++)
        {
            categoriesToReview[i].gameObject.SetActive(true);
        }
    }
    ///////////////////////////////////////////////
    private void FindButtonCategories()
    {
        buttonCategories.Clear();

        foreach (Image child in sectors[indexSector].GetComponentsInChildren<Image>())
        {
            if (child.name.Contains("B_") && child != transform)
            {
                buttonCategories.Add(child);
            }
        }

        SelectCategory(-1);
    }
    ///////////////////////////////////////////////
    private void FindSelectables()
    {
        allSelectables.Clear();

        foreach (Selectable child in sectors[indexSector].GetComponentsInChildren<Selectable>())
        {
            if (child.name.Contains("S_") && child != transform)
            {
                allSelectables.Add(child);
            }
        }

        allSelectables[0].Select();
    }
    private void FindCategories()
    {
        categoriesToReview.Clear();

        foreach (RectTransform child in sectors[indexSector].GetComponentsInChildren<RectTransform>())
        {
            if (child.name.Contains("Category_") && child != transform)
            {
                categoriesToReview.Add(child);
            }
        }

        if (categoriesToReview.Count == 0)
        {
            Debug.Log("No se han encontrado categorías en " + sectors[indexSector].name);
            return;
        }

        FindButtonCategories();
    }
    private void SelectCategory(int num)
    {
        if (num < 0)
        {
            for(int i = 0; i < categoriesToReview.Count; i++)
            {
                categoriesToReview[i].gameObject.SetActive(false);
                buttonCategories[i].color = unselectedCategory;
            }

            indexCategory = 0;
        }
        else
        {
            categoriesToReview[indexCategory].gameObject.SetActive(false);

            buttonCategories[indexCategory].color = unselectedCategory;

            indexCategory = num;
        }

        categoriesToReview[indexCategory].gameObject.SetActive(true);
        buttonCategories[indexCategory].color = selectedCategory;

        FindSelectables();

        DefineSelector(categoriesToReview[indexCategory].transform);
    }
}
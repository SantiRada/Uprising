using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventorySystem : MonoBehaviour {

    public int countMaxSlots = 0;
    public int[] mineralIndex;
    [HideInInspector] public int countFilledSlots = 0;

    [SerializeField] private int[] mineralCounter;
    private bool isOpenInventory = true;

    [Header("UI Content")]
    public GameObject viewInventory;
    public TextMeshProUGUI textItems;
    public TextMeshProUGUI textCounters;

    [Header("Animation Add Mineral")]
    public TextMeshProUGUI textAddMineral;
    public float delayAnim;
    public List<string> queueMineral = new List<string>();
    private InputPlayerSystem _inputs;

    private void Awake()
    {
        _inputs = GetComponent<InputPlayerSystem>();
    }
    private void Start()
    {
        mineralCounter = new int[mineralIndex.Length];

        viewInventory.gameObject.SetActive(false);
        textItems.text = "";
        textCounters.text = "";
        textAddMineral.text = "";

        _inputs.useInventory += ChangeInventory;
        _inputs.useEscape += CloseInventory;
        _inputs.interact += ClicUser;
    }
    private void CloseInventory()
    {
        if (isOpenInventory)
        {
            isOpenInventory = false;

            PauseMenu.SetState(StatePlayer.Game);

            viewInventory.gameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        _inputs.useInventory -= ChangeInventory;
        _inputs.interact -= ClicUser;

        _inputs.useEscape -= CloseInventory;
    }
    private void ClicUser()
    {
        if (PauseMenu.state == StatePlayer.Pause) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                if (hit.transform.gameObject.CompareTag("Mineral")) AddMineral(hit.transform.gameObject);
            }
        }
    }
    private void ChangeInventory()
    {
        if (PauseMenu.state == StatePlayer.Pause) return;

        if (!isOpenInventory)
        {
            isOpenInventory = true;

            viewInventory.gameObject.SetActive(true);
            PauseMenu.SetState(StatePlayer.UserInterface);

            viewInventory.transform.SetAsLastSibling();
        }
        else
        {
            isOpenInventory = false;

            PauseMenu.SetState(StatePlayer.Game);

            viewInventory.gameObject.SetActive(false);
        }
    }
    private void AddMineral(GameObject obj)
    {
        if(countFilledSlots >= countMaxSlots)
        {
            queueMineral.Add(LanguageSystem.GetValue("game", 16));
            StartCoroutine("AnimationAddMineral");
            return;
        }

        countFilledSlots++;
        int id = int.Parse(obj.name.Split('-')[1]);

        for (int i = 0; i < mineralIndex.Length; i++)
        {
            if (id == mineralIndex[i])
            {
                mineralCounter[i]++;
                Destroy(obj);
                break;
            }
        }

        textItems.text = "";
        textCounters.text = "";
        for(int i = 0; i < mineralIndex.Length; i++)
        {
            if (mineralCounter[i] > 0)
            {
                queueMineral.Add((LanguageSystem.GetValue("game", 11)) + " " + LanguageSystem.GetValue("game", mineralIndex[i]) + "\n");

                textItems.text += LanguageSystem.GetValue("game", mineralIndex[i]) + "\n";
                textCounters.text += "x" + mineralCounter[i] + "\n";
                StartCoroutine("AnimationAddMineral");
            }
        }

        if(countFilledSlots >= countMaxSlots) queueMineral.Add(LanguageSystem.GetValue("game", 16) + "\n");
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Mineral")) AddMineral(collision.gameObject);
    }
    private IEnumerator AnimationAddMineral()
    {
        textAddMineral.text = "";

        for(int i = 0; i < queueMineral.Count; i++)
        {
            textAddMineral.text += queueMineral[i];
        }

        yield return new WaitForSeconds(delayAnim);
        if(queueMineral.Count > 0) queueMineral.RemoveAt(0);

        if (queueMineral.Count > 0) { StartCoroutine("AnimationAddMineral"); }
        else
        {
            queueMineral.Clear();
            textAddMineral.text = "";
        }
    }
    public int[] GetInventory()
    {
        return mineralCounter;
    }
    public void VoidInventory(int maxValue)
    {
        for(int i = 0; i < maxValue; i++)
        {
            mineralCounter[i] = 0;
        }

        countFilledSlots = 0;
    }
    public void ReviewInventory()
    {
        textItems.text = "";
        textCounters.text = "";
        countFilledSlots = 0;

        for (int i = 0; i < mineralCounter.Length; i++)
        {
            if (mineralCounter[i] > 0)
            {
                textItems.text += LanguageSystem.GetValue("game", mineralIndex[i]) + "\n";
                textCounters.text += "x" + mineralCounter[i] + "\n";
                countFilledSlots += mineralCounter[i];
            }
        }
    }
    public void AddMineral(int pos, bool isSum = true)
    {
        if (isSum) mineralCounter[pos]++;
        else mineralCounter[pos]--;

        ReviewInventory();
    }
}
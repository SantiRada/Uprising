using TMPro;
using UnityEngine;

public class FuelSystem : MonoBehaviour {

    [Header("Values Fuel")]
    public int amountNecessary;
    public int[] mineralCounter = new int[10];
    [SerializeField] private int currentAmount;

    [Header("UI System")]
    [Tooltip("Distancia a la izquierda donde debe colocarse el inventario")] public float offsetX = -285;
    public GameObject inventoryObject;
    public TextMeshProUGUI[] numberMineral;
    public TextMeshProUGUI amountUI;

    private InventorySystem _player;
    private InteractSystem _interaction;

    private void Awake()
    {
        _player = FindAnyObjectByType<InventorySystem>();
        _interaction = GetComponent<InteractSystem>();
    }
    private void Start()
    {
        mineralCounter = new int[_player.mineralIndex.Length];

        for(int i = 0; i < mineralCounter.Length; i++)
        {
            numberMineral[i].text = mineralCounter[i].ToString();
        }
        amountUI.text = currentAmount.ToString() + "/" + amountNecessary.ToString();

        _interaction.actionOpenUI += OpenInventory;
        _interaction.actionCloseUI+= CloseInventory;
    }
    private void OpenInventory()
    {
        inventoryObject.gameObject.SetActive(true);
        inventoryObject.GetComponent<RectTransform>().SetLocalPositionAndRotation(new Vector3(offsetX, 0, 0), Quaternion.identity);
    }
    private void CloseInventory()
    {
        inventoryObject.GetComponent<RectTransform>().SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
        inventoryObject.gameObject.SetActive(false);
    }
    public void RemoveMineral(int pos)
    {
        if (mineralCounter[pos] <= 0) return;

        mineralCounter[pos]--;

        numberMineral[pos].text = mineralCounter[pos].ToString();

        _player.AddMineral(pos, true);
    }
    public void AddMineral(int pos)
    {
        if (_player.GetInventory()[pos] <= 0 || currentAmount >= amountNecessary) return;

        mineralCounter[pos]++;
        
        numberMineral[pos].text = mineralCounter[pos].ToString();

        _player.AddMineral(pos, false);
    }
    public void ConvertToFuel()
    {
        if(currentAmount >= amountNecessary) return;

        int count = 0;
        for (int i = 0; i < mineralCounter.Length; i++)
        {
            count += mineralCounter[i];

            mineralCounter[i] = 0;
            numberMineral[i].text = mineralCounter[i].ToString();
        }

        currentAmount += count;
        if (currentAmount > amountNecessary) currentAmount = amountNecessary;

        // Change UI
        amountUI.text = currentAmount.ToString() +  "/" + amountNecessary.ToString();
    }
    private void OnDestroy()
    {
        _interaction.actionOpenUI -= OpenInventory;
        _interaction.actionCloseUI -= CloseInventory;
    }
}
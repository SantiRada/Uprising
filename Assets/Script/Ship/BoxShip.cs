using TMPro;
using UnityEngine;

public class BoxShip : MonoBehaviour {

    [Header("Box Ship")]
    public TextMeshProUGUI[] numberBox;
    public TextMeshProUGUI filledText;

    [Header("Minerales")]
    public int countMaxSlots = 0;
    public int countFilledSlots = 0;
    [SerializeField] private int[] mineralCounter;

    private InventorySystem _player;

    private void Awake()
    {
        _player = FindAnyObjectByType<InventorySystem>();

        mineralCounter = new int[_player.mineralIndex.Length];
    }
    private void Start()
    {
        ReloadMinerals();
    }
    public void SaveMinerals()
    {
        int[] allInventory = _player.GetInventory();

        for (int i = 0; i < allInventory.Length; i++)
        {
            if ((countFilledSlots + allInventory[i]) < countMaxSlots)
            {
                countFilledSlots += allInventory[i];

                mineralCounter[i] += allInventory[i];

                _player.VoidInventory(i);
            }
            else
            {
                filledText.text = "¡Inventario Lleno!";
                Invoke("HideFilled", 1.5f);
            }
        }

        _player.ReviewInventory();
        ReloadMinerals();
    }
    private void HideFilled()
    {
        filledText.gameObject.SetActive(false);
    }
    private void ReloadMinerals()
    {
        countFilledSlots = 0;

        for(int i = 0; i< numberBox.Length; i++)
        {
            numberBox[i].text = mineralCounter[i].ToString();
            countFilledSlots += mineralCounter[i];
        }

        filledText.text = countFilledSlots + " / " + countMaxSlots;
    }
    public void RestarMineral(int pos)
    {
        if (mineralCounter[pos] > 0)
        {
            if (_player.countFilledSlots >= _player.countMaxSlots) return;

            _player.AddMineral(pos);

            mineralCounter[pos]--;

            ReloadMinerals();
        }
    }
}
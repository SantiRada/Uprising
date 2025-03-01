using UnityEngine;

public enum Rareza { Comun = 0, PocoComun = 1, Raro = 2, Epico = 3, Legendario = 4 }
[CreateAssetMenu(fileName = "Mineral", menuName = "Scriptable/Mineral")]
public class Mineral : ScriptableObject {

    public int id;
    public GameObject mineral;

    public Vector2Int rangeHeight = new Vector2Int(0, 10);
    public int countMaxToCreate;
    public int probability = 10;
    public Rareza rareza;

    public int GetMinHeight() { return -rangeHeight.x; }
    public int GetMaxHeight() { return -rangeHeight.y; }
}
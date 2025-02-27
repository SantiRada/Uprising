using UnityEngine;

[CreateAssetMenu(fileName = "Mineral", menuName = "Scriptable")]
public class Mineral : ScriptableObject {

    public int id;
    public GameObject mineral;

    public Vector2Int rangeHeight = new Vector2Int(0, 10);
    public int maxPoint = 5;
    public int probability = 10;

    public int GetMinHeight() { return -rangeHeight.x; }
    public int GetMaxHeight() { return -rangeHeight.y; }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Planet", menuName = "Scriptable/Planet")]
public class PlanetScriptable : ScriptableObject {

    public int namePlanet;
    public GameObject prefabPlanet;
    [Tooltip("La rareza del planeta define su nivel estructural")] public Rareza rarePlanet;

    [Tooltip("Mineral que le PIDE al jugador")] public Mineral receiveMineral;
    [Tooltip("Mineral que le DA al jugador")] public Mineral givesMineral;
}

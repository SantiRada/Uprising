using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadMapFactions : MonoBehaviour {

    public GameObject prefabFaction;
    public Sprite dottedLineSprite; // Imagen de línea punteada
    public float minDistance;
    public float lineWidth = 5f;

    private List<RectTransform> factionChildrens = new List<RectTransform>();
    private List<LineRenderer> lines = new List<LineRenderer>();

    private Vector2 size;
    private PlanetarySystem _planetary;

    private void Awake()
    {
        _planetary = FindAnyObjectByType<PlanetarySystem>();
    }
    private void Start()
    {
        size = GetComponent<RectTransform>().sizeDelta / 2;
        CreateFactionInMap();
        DrawDashedLines();
    }
    private void CreateFactionInMap()
    {
        for (int i = 0; i < _planetary.dataPlanets.Count; i++)
        {
            Vector3 newPosition = GetValidPosition();

            RectTransform newFaction = Instantiate(prefabFaction, Vector3.zero, Quaternion.identity, transform).GetComponent<RectTransform>();
            factionChildrens.Add(newFaction);
            newFaction.SetLocalPositionAndRotation(newPosition, Quaternion.identity);

            MapPoint point = newFaction.GetComponent<MapPoint>();
            point.nameFaction = _planetary.dataPlanets[i].namePlanet;

            point.planetObj = _planetary.dataPlanets[i].prefabPlanet;
            point.factionObj = _planetary.dataPlanets[i].prefabPlanet;
        }
    }
    private Vector3 GetValidPosition()
    {
        Vector3 newPosition;
        int maxAttempts = 100; // Evita loops infinitos
        int attempts = 0;

        do
        {
            newPosition = new Vector3(
                Random.Range(-(size.x - 70), (size.x - 70)),
                Random.Range(-(size.y - 70), (size.y - 70)),
                0
            );

            attempts++;
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Se alcanzó el límite de intentos, reduciendo la restricción de distancia.");
                break;
            }

        } while (!IsValidPosition(newPosition));

        return newPosition;
    }
    private bool IsValidPosition(Vector3 position)
    {
        foreach (var faction in factionChildrens)
        {
            if (Vector3.Distance(position, faction.anchoredPosition) < minDistance)
            {
                return false; // La posición está demasiado cerca de otro punto
            }
        }
        return true;
    }
    private void DrawDashedLines()
    {
        for (int i = 1; i < factionChildrens.Count; i++)
        {
            CreateDashedLine(factionChildrens[(i - 1)], factionChildrens[i]);
        }
    }
    private void CreateDashedLine(RectTransform start, RectTransform end)
    {
        GameObject lineObj = new GameObject("DashedLine", typeof(Image));
        lineObj.transform.SetParent(transform, false);

        Image lineImage = lineObj.GetComponent<Image>();
        lineImage.sprite = dottedLineSprite;
        lineImage.type = Image.Type.Tiled; // Repite la textura
        lineImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Posición y tamaño de la línea
        Vector2 startPos = start.anchoredPosition;
        Vector2 endPos = end.anchoredPosition;
        Vector2 direction = (endPos - startPos).normalized;
        float distance = Vector2.Distance(startPos, endPos);

        lineImage.rectTransform.sizeDelta = new Vector2(distance, lineWidth);
        lineImage.rectTransform.anchoredPosition = (startPos + endPos) / 2;

        lineImage.rectTransform.SetAsFirstSibling();

        // Rotación de la línea
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineImage.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
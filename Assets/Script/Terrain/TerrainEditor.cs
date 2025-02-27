using UnityEngine;

public class TerrainEditor : MonoBehaviour {

    [Range(0.6f, 3f)] public float excavationRadius = 3f;
    [Range(0f, 1f)] public float forceExcavation = 0.7f;
    [Range(0f, 2f)] public float delayToClic;
    public float rangeToClic;
    private float _baseDelay;

    private TerrainGenerator terrainGen;
    private InputPlayerSystem _inputs;

    private void Awake()
    {
        _inputs = FindAnyObjectByType<InputPlayerSystem>();

        _baseDelay = delayToClic;
        delayToClic = 0f;
    }
    private void Start()
    {
        terrainGen = GetComponent<TerrainGenerator>();

        _inputs.excavation += ClicUser;
    }
    private void OnDestroy()
    {
        _inputs.excavation -= ClicUser;
    }
    private void Update()
    {
        if(delayToClic > 0) delayToClic -= Time.deltaTime;
    }
    private void ClicUser()
    {
        if (delayToClic > 0) return;

        delayToClic = _baseDelay;

        Transform camTransform = Camera.main.transform;
        Vector3 rayDirection = camTransform.forward;

        if (_inputs.moveCamera != Vector3.zero)
        {
            Vector3 right = camTransform.right * _inputs.moveCamera.x;
            Vector3 up = camTransform.up * _inputs.moveCamera.y;

            rayDirection += right + up;
            rayDirection.Normalize();
        }

        Ray ray = new Ray(camTransform.position, rayDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (Vector3.Distance(hit.point, camTransform.position) < rangeToClic)
            {
                ExcavateAtPoint(hit.point);
            }
        }
    }

    private void ExcavateAtPoint(Vector3 worldPoint)
    {
        Vector3 localPoint = worldPoint - terrainGen.transform.position;

        int sizeX = terrainGen.size.x;
        int sizeY = terrainGen.size.y;
        int sizeZ = terrainGen.size.z;

        int dimX = sizeX + 1;
        int dimY = sizeY + 1;
        int dimZ = sizeZ + 1;

        float[,,] density = terrainGen.density;
        if (density == null)
        {
            Debug.LogError("El array de densidad es nulo.");
            return;
        }

        int minX = Mathf.Max(0, Mathf.FloorToInt(localPoint.x - excavationRadius));
        int maxX = Mathf.Min(dimX - 1, Mathf.CeilToInt(localPoint.x + excavationRadius));
        int minY = Mathf.Max(0, Mathf.FloorToInt(localPoint.y - excavationRadius));
        int maxY = Mathf.Min(dimY - 1, Mathf.CeilToInt(localPoint.y + excavationRadius));
        int minZ = Mathf.Max(0, Mathf.FloorToInt(localPoint.z - excavationRadius));
        int maxZ = Mathf.Min(dimZ - 1, Mathf.CeilToInt(localPoint.z + excavationRadius));

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    float distance = Vector3.Distance(pos, localPoint);
                    if (distance <= excavationRadius) density[x, y, z] = -forceExcavation;
                }
            }
        }

        terrainGen.GenerateMesh();
    }
}
using System.Collections.Generic;
using UnityEngine;

public class CreatorOfChunks : MonoBehaviour {

    [Header("Distances")]
    [Range(0, 10)] public float distanceToNewChunk;
    public int offset = 1;

    [Header("Chunks")]
    public Vector3Int sizeChunk;
    public GameObject chunkPrefab;
    public Material[] listMaterial;
    public int[] positionChangeMaterial;
    [HideInInspector] public List<TerrainGenerator> chunks = new List<TerrainGenerator>();

    [Header("Private Content")]
    private int counterGrounds = 0;
    private int numMaterial = 0;
    private float minY = 0;
    private Player _player;
    private MineralGenerator _mineralGenerator;

    private void Awake()
    {
        _player = FindAnyObjectByType<Player>();
        _mineralGenerator = GetComponent<MineralGenerator>();
    }
    private void Start() { CreateGround(); }
    private void Update()
    {
        if (_player.transform.position.y < minY)
        {
            minY = _player.transform.position.y;
            int minValue = -(counterGrounds * sizeChunk.y);

            if ((minY - minValue) < distanceToNewChunk) CreateGround();
        }
    }
    private void CreateGround()
    {
        if((int)_player.transform.position.y < -positionChangeMaterial[numMaterial])
        {
            if ((listMaterial.Length - 1) > numMaterial) numMaterial++;
            else
            {
                int half = listMaterial.Length / 2;
                int rnd = Random.Range(0, half);

                if(rnd >= (half / 2)) numMaterial -= rnd;
            }
        }

        for(int i = 0; i < 9; i++)
        {
            switch (i)
            {
                case 0: CreateChunk(-1, 0, 1); break;
                case 1: CreateChunk(0, 0, 1); break;
                case 2: CreateChunk(1, 0, 1); break;
                case 3: CreateChunk(-1, 0, 0); break;
                case 4: CreateChunk(0, 0, 0); break;
                case 5: CreateChunk(1, 0, 0); break;
                case 6: CreateChunk(-1, 0, -1); break;
                case 7: CreateChunk(0, 0, -1); break;
                case 8: CreateChunk(1, 0, -1); break;
            }
        }
        counterGrounds++;
    }
    private void CreateChunk(int x, int y, int z)
    {
        x *= (sizeChunk.x - offset);
        y = -(sizeChunk.y * (counterGrounds + 1) - (offset * counterGrounds));
        z *= (sizeChunk.z - offset);
        Vector3Int newPosition = new Vector3Int(x, y, z);

        GameObject chunk = Instantiate(chunkPrefab, newPosition, Quaternion.identity);
        
        MeshRenderer renderer = chunk.GetComponent<MeshRenderer>();
        renderer.material = listMaterial[numMaterial];

        TerrainGenerator terrain = chunk.GetComponent<TerrainGenerator>();
        terrain.size = sizeChunk;

        _mineralGenerator.CreateMineral(terrain, (terrain.transform.position.y / 2));

        chunks.Add(terrain);
    }
}
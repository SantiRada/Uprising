using UnityEngine;

public class MineralGenerator : MonoBehaviour {

    public Mineral[] listMineral;
    public int[] counterMineral;

    private void Start()
    {
        counterMineral = new int[listMineral.Length];
    }
    public void CreateMineral(TerrainGenerator terrain, float pos)
    {
        for(int i = 0; i < listMineral.Length; i++)
        {
            if(pos < listMineral[i].GetMinHeight() && pos > listMineral[i].GetMaxHeight())
            {
                counterMineral[i] += GenerateMineralInChunk(listMineral[i], terrain);
            }
        }
    }
    private int GenerateMineralInChunk(Mineral mineral, TerrainGenerator terrain)
    {
        int mineralCount = 0;

        for (int i = 0; i < mineral.countMaxToCreate; i++)
        {
            if (Random.Range(0, 100) >= mineral.probability)
                continue;

            Vector3 spawnPos = new Vector3(
                Random.Range(terrain.transform.position.x, terrain.transform.position.x + terrain.size.x),
                Random.Range(terrain.transform.position.y, terrain.transform.position.y + terrain.size.y),
                Random.Range(terrain.transform.position.z, terrain.transform.position.z + terrain.size.z)
            );

            GameObject min = Instantiate(mineral.mineral, spawnPos, Quaternion.identity);
            min.transform.SetParent(transform, false);
            min.name = "Mineral-" + mineral.id;
            mineralCount++;
        }

        return mineralCount;
    }
}
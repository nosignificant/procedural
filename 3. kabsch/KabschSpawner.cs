using UnityEngine;

public class KabschSpawner : MonoBehaviour
{
    public GameObject refPrefab;
    public GameObject inPrefab;

    public GameObject weedPrefab;

    public float refSpawnRadius = 100f;
    public float inSpawnRadius = 100f;
    public int spawnCount = 100;

    public float initSpeed = 2f;

    public void SpawnerMove(Vector3 avgPos)
    {
        transform.position = avgPos;
    }

    public void Spawn(out Transform[] generatedRefs, out Transform[] generatedIns)
    {
        generatedRefs = new Transform[spawnCount];
        generatedIns = new Transform[spawnCount];

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 refRandomPos = Random.insideUnitSphere * refSpawnRadius;
            Vector3 inRandomPos = Random.insideUnitSphere * inSpawnRadius;


            GameObject r = Instantiate(refPrefab, refRandomPos, Quaternion.identity);

            GameObject n = Instantiate(inPrefab, inRandomPos, Quaternion.identity);

            generatedRefs[i] = r.transform;
            generatedIns[i] = n.transform;

            r.transform.parent = this.transform;
            n.transform.parent = this.transform;

            if (weedPrefab != null)
            {
                SpawnWeed(n.transform, inRandomPos);
            }
        }
    }

    void SpawnWeed(Transform inChild, Vector3 spawnPos)
    {
        Vector3 weedPos = spawnPos + new Vector3(0, 0, -2);

        GameObject w = Instantiate(weedPrefab, weedPos, Quaternion.identity);
        w.transform.parent = this.transform;

        Weed3 weed = w.GetComponent<Weed3>();
        if (weed != null)
            weed.SetTarget(inChild);
    }
}
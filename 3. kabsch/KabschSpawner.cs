using UnityEngine;

public class KabschSpawner : MonoBehaviour
{
    public GameObject refPrefab;
    public GameObject inPrefab;

    public float refSpawnRadius = 100f;
    public float inSpawnRadius = 100f;

    public int spawnCount = 100;

    public float initSpeed = 2f;

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
        }
    }
}
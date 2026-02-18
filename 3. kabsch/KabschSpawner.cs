using UnityEngine;

public class KabschSpawner : MonoBehaviour
{
    // boid, 발 전체 총괄 소환
    public GameObject refPrefab;
    public GameObject inPrefab;

    public GameObject weedPrefab;

    public GameObject legPrefab;

    public Transform center;

    public float refSpawnRadius = 100f;
    public float inSpawnRadius = 100f;
    public int spawnCount = 100;

    public float initSpeed = 2f;



    public void getKabschCenter(Transform avgPos)
    {
        center = avgPos;
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

            if (legPrefab != null)
            {
                SpawnLeg(r.transform, n.transform);
            }


        }
        if (weedPrefab != null)
            SpawnWeed();
    }

    void SpawnWeed()
    {
        if (center == null)
        {
            Debug.LogWarning("Kabsch Center가 설정되지 않아 Weed를 생성할 수 없습니다!");
            return;
        }
        Vector3 weedPos = center.position + new Vector3(0, 0, -2);

        GameObject w = Instantiate(weedPrefab, weedPos, Quaternion.identity);
        w.transform.parent = this.transform;

        Leg leg = w.GetComponent<Leg>();
        if (leg != null)
            leg.SetTarget(center);
    }

    void SpawnLeg(Transform r, Transform n)
    {
        if (center == null)
        {
            Debug.LogWarning("Kabsch Center가 설정되지 않아 Weed를 생성할 수 없습니다!");
            return;
        }
        GameObject w = Instantiate(legPrefab, r);

        float randomSize = Random.Range(0.3f, 0.7f);
        w.transform.localScale = Vector3.one * randomSize;

        CCDIK ccdik = w.GetComponent<CCDIK>();
        if (ccdik != null) ccdik.SetOrbitTarget(n);

    }
}
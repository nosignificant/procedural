using UnityEngine;

public class Kabsch : MonoBehaviour
{
    public int iteration = 5;

    //현재 transfrom
    public Transform[] refChild;
    public Transform[] inChild;

    Vector3[] inPoint; Vector3[] refPoint;

    public Transform target;
    public Transform parent;

    public GameObject centerPrefab;
    GameObject centerInstance;

    float moveSpeed = 5.0f;

    public Vector3 avgRefPos = new Vector3();
    public Vector3 avgInPos = new Vector3();


    void Start()
    {
        inPoint = new Vector3[inChild.Length];
        refPoint = new Vector3[refChild.Length];

        for (int i = 0; i < inChild.Length; i++)
        {
            inPoint[i] = inChild[i].position;
        }
        centerInstance = Instantiate(centerPrefab, parent);

    }

    void Update()
    {
        Quaternion optimalRot = Kab();

        if (centerInstance != null)
            centerInstance.transform.position = avgRefPos;

        //transform.position =
        //Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        for (int i = 0; i < inChild.Length; i++)
        {
            // 공식: 목표중심 + (회전 * (내위치 - 내중심))
            Vector3 relativePos = inChild[i].position - avgInPos; // 내 중심 빼기
            Vector3 rotatedPos = optimalRot * relativePos;        // 회전 시키기
            Vector3 finalPos = avgRefPos + rotatedPos;            // 목표 중심으로 이동

            // 부드럽게 이동
            inChild[i].position = Vector3.Lerp(inChild[i].position, finalPos, moveSpeed * Time.deltaTime);
        }

    }

    Quaternion Kab()
    {
        if (refChild.Length != inChild.Length) return Quaternion.identity;
        Quaternion OptimalRotation = Quaternion.identity;

        avgRefPos = Vector3.zero;
        avgInPos = Vector3.zero;

        // 1. 이동 평균 //
        for (int i = 0; i < refChild.Length; i++)
        {
            avgRefPos += refChild[i].position;
            avgInPos += inChild[i].position;

            refPoint[i] = refChild[i].position;
            inPoint[i] = inChild[i].position;
        }

        avgRefPos = avgRefPos / refChild.Length;
        avgInPos = avgInPos / refChild.Length;

        // 2. 회전 평균 //
        //1) 평균 위치로 옮기기
        Vector3[] cov = TransposeAndCov(inPoint, refPoint, avgInPos, avgRefPos);

        extractRotation(cov, ref OptimalRotation);

        return OptimalRotation;
    }

    //2) 공분산 구하기
    Vector3[] TransposeAndCov(Vector3[] inV, Vector3[] refV, Vector3 inAvg, Vector3 refAvg)
    {
        Vector3[] cov = new Vector3[3];

        for (int k = 0; k < inV.Length; k++)
        {
            Vector3 ptIn = inV[k] - inAvg;
            Vector3 ptRef = refV[k] - refAvg;

            // 3x3 행렬 누적
            cov[0][0] += ptIn.x * ptRef.x;
            cov[0][1] += ptIn.x * ptRef.y;
            cov[0][2] += ptIn.x * ptRef.z;

            cov[1][0] += ptIn.y * ptRef.x;
            cov[1][1] += ptIn.y * ptRef.y;
            cov[1][2] += ptIn.y * ptRef.z;

            cov[2][0] += ptIn.z * ptRef.x;
            cov[2][1] += ptIn.z * ptRef.y;
            cov[2][2] += ptIn.z * ptRef.z;
        }
        return cov;
    }

    //3) 회전 구하기
    void extractRotation(Vector3[] cov, ref Quaternion q)
    {
        Vector3 omega = new Vector3();

        for (int i = 0; i < iteration; i++)
        {
            Vector3 xDir = q * Vector3.right; // (1, 0, 0)
            Vector3 yDir = q * Vector3.up; // (0, 1, 0)
            Vector3 zDir = q * Vector3.forward; //(0, 0, 1)

            //회전해야할 양
            Vector3 crossSum = Vector3.Cross(xDir, cov[0])
                + Vector3.Cross(yDir, cov[1])
                + Vector3.Cross(zDir, cov[2]);
            float dotSum = Mathf.Abs(Vector3.Dot(xDir, cov[0])
                + Vector3.Dot(yDir, cov[1])
                + Vector3.Dot(zDir, cov[2]));

            omega = crossSum / dotSum;

            float w = omega.magnitude;
            if (w < 0.000001) break;

            q = Quaternion.AngleAxis(w * Mathf.Rad2Deg, omega.normalized) * q;
            q = Quaternion.Lerp(q, q, 0f);
        }
    }
}
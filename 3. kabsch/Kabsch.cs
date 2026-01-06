using UnityEngine;

public class Kabsch : MonoBehaviour
{
    [Header("Settings")]
    public int iteration = 9;

    [Header("Rotation Limit (New)")]
    public bool enableLimit = true;       // 회전 제한 켜기/끄기
    [Range(0, 180)]
    public float maxRotationAngle = 180f;  // ★ 최대 45도까지만 회전 허용 (이거 넘어가면 45도에 고정)

    [Header("Targets")]
    public Transform target;

    [Header("Objects")]
    public Transform[] refChild;
    public Transform[] inChild;

    [Header("Debug Visualizer")]
    public GameObject centerPrefab;
    public Transform parent;
    private GameObject centerInstance;

    // 내부 변수들
    private Vector3[] originalInLocalPos;
    [HideInInspector] public Vector3 avgRefPos;
    private Vector3 avgInPos;
    private Vector3[] currentRefPoints;

    void Start()
    {
        originalInLocalPos = new Vector3[inChild.Length];
        currentRefPoints = new Vector3[refChild.Length];

        Vector3 sumIn = Vector3.zero;
        for (int i = 0; i < inChild.Length; i++) sumIn += inChild[i].position;
        avgInPos = sumIn / inChild.Length;

        for (int i = 0; i < inChild.Length; i++)
        {
            originalInLocalPos[i] = inChild[i].position - avgInPos;
        }

        if (centerPrefab != null)
        {
            centerInstance = Instantiate(centerPrefab, parent != null ? parent : transform);
        }
    }

    void Update()
    {
        CalculateRefCenter();

        if (centerInstance != null) centerInstance.transform.position = avgRefPos;

        // 1. 최적 회전 계산
        Quaternion optimalRot = SolveKabsch();

        // ★ 2. 회전 각도 제한 로직 (New) ★
        if (enableLimit)
        {
            // 기본 상태(Identity)와 계산된 회전(optimalRot) 사이의 각도 차이 계산
            float angle = Quaternion.Angle(Quaternion.identity, optimalRot);

            // 만약 각도가 제한치(maxRotationAngle)보다 크다면?
            if (angle > maxRotationAngle)
            {
                // 강제로 제한 각도까지만 회전시킨 값으로 덮어씌움
                optimalRot = Quaternion.RotateTowards(Quaternion.identity, optimalRot, maxRotationAngle);
            }
        }

        // 3. 제한된 회전 적용
        ApplyToInChildren(optimalRot);
    }

    void CalculateRefCenter()
    {
        Vector3 sumRef = Vector3.zero;
        for (int i = 0; i < refChild.Length; i++)
        {
            currentRefPoints[i] = refChild[i].position;
            sumRef += currentRefPoints[i];
        }
        avgRefPos = sumRef / refChild.Length;
    }

    void ApplyToInChildren(Quaternion rot)
    {
        for (int i = 0; i < inChild.Length; i++)
        {
            Vector3 targetPos = avgRefPos + (rot * originalInLocalPos[i]);
            inChild[i].position = Vector3.Lerp(inChild[i].position, targetPos, 0.05f);
        }
    }

    Quaternion SolveKabsch()
    {
        if (refChild.Length != inChild.Length) return Quaternion.identity;
        Vector3[] cov = TransposeAndCov(originalInLocalPos, currentRefPoints, Vector3.zero, avgRefPos);
        Quaternion q = Quaternion.identity;
        extractRotation(cov, ref q);
        return q;
    }

    Vector3[] TransposeAndCov(Vector3[] inRel, Vector3[] refAbs, Vector3 inCenterRel, Vector3 refCenterAbs)
    {
        Vector3[] cov = new Vector3[3];
        for (int k = 0; k < inRel.Length; k++)
        {
            Vector3 ptIn = inRel[k];
            Vector3 ptRef = refAbs[k] - refCenterAbs;
            cov[0][0] += ptIn.x * ptRef.x; cov[0][1] += ptIn.x * ptRef.y; cov[0][2] += ptIn.x * ptRef.z;
            cov[1][0] += ptIn.y * ptRef.x; cov[1][1] += ptIn.y * ptRef.y; cov[1][2] += ptIn.y * ptRef.z;
            cov[2][0] += ptIn.z * ptRef.x; cov[2][1] += ptIn.z * ptRef.y; cov[2][2] += ptIn.z * ptRef.z;
        }
        return cov;
    }

    void extractRotation(Vector3[] cov, ref Quaternion q)
    {
        Vector3 omega = new Vector3();
        for (int i = 0; i < iteration; i++)
        {
            Vector3 xDir = q * Vector3.right; Vector3 yDir = q * Vector3.up; Vector3 zDir = q * Vector3.forward;
            Vector3 crossSum = Vector3.Cross(xDir, cov[0]) + Vector3.Cross(yDir, cov[1]) + Vector3.Cross(zDir, cov[2]);
            float dotSum = Mathf.Abs(Vector3.Dot(xDir, cov[0]) + Vector3.Dot(yDir, cov[1]) + Vector3.Dot(zDir, cov[2])) + 1e-6f;
            omega = crossSum / dotSum;
            float w = omega.magnitude;
            if (w < 1e-6f) break;
            q = Quaternion.AngleAxis(w * Mathf.Rad2Deg, omega.normalized) * q;
            q = q.normalized;
        }
    }
}
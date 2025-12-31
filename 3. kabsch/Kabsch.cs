using UnityEngine;

public class Kabsch : MonoBehaviour
{
    public int iteration = 5;

    //현재 transfrom
    Transform[] children;

    //태초의 transform
    Vector3[] originalRel;

    int childCount = 0;
    Vector3 avgPos = new Vector3();

    void Start()
    {

        childCount = transform.childCount;
        children = new Transform[childCount];
        originalRel = new Vector3[childCount];

        for (int i = 0; i < childCount; i++)
        {
            children[i] = transform.GetChild(i);
            originalRel[i] = children[i].localPosition;
        }
    }

    void Update()
    {
        Kab();
    }

    void Kab()
    {
        Vector3[] tempChild = new Vector3[childCount];

        //1. 이동 평균
        for (int i = 0; i < childCount; i++)
        {
            tempChild[i] = children[i].position;
            avgPos += tempChild[i];
        }
        avgPos = avgPos / childCount;

        //2. 회전 평균

        //1) 평균 위치로 옮기기
        for (int i = 0; i < childCount; i++)
            tempChild[i] -= avgPos;

        //2) 공분산 구하기
        Vector3[] cov = new Vector3[3];

        for (int k = 0; k < childCount; k++)
        {
            Vector3 rot = transform.rotation * originalRel[k];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    //orignalRel은 태초의 상태이기 때문에, 현재 상태를 반영해줘야 한다.
                    cov[i][j] += rot[i] * tempChild[k][j];
                }
            }
        }

        //3) 회전 구하기
        Quaternion currentRot = transform.rotation;
        Vector3 omega = new Vector3();

        for (int i = 0; i < iteration; i++)
        {
            Vector3 xDir = currentRot * Vector3.right; // (1, 0, 0)
            Vector3 yDir = currentRot * Vector3.up; // (0, 1, 0)
            Vector3 zDir = currentRot * Vector3.forward; //(0, 0, 1)

            //회전해야할 양
            Vector3 crossSum = Vector3.Cross(xDir, cov[0]) + Vector3.Cross(yDir, cov[1]) + Vector3.Cross(zDir, cov[2]);
            float dotSum = Mathf.Abs(Vector3.Dot(xDir, cov[0]) + Vector3.Dot(yDir, cov[1]) + Vector3.Dot(zDir, cov[2]));
            omega = crossSum / dotSum;

            float w = omega.magnitude;
            if (w < 0.000001) break;

            currentRot = Quaternion.AngleAxis(w * Mathf.Rad2Deg, omega.normalized) * currentRot;
        }
        transform.rotation = currentRot;
    }

}
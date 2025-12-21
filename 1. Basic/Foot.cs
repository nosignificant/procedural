using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Foot : MonoBehaviour
{
    public Transform root;
    public Vector3 stablePosition;
    public Vector3 defaultPos;


    //0이 왼쪽, 1이 오른쪽
    public int LR;
    public LayerMask groundLayer;

    private float theta;

    void Start()
    {
        stablePosition = this.transform.position;

        //몸통 대비 발의 상대 위치 
        defaultPos = this.transform.position - root.position;

        //몸통 - 발의 초기 각도 
        theta = Vector2.SignedAngle(
        new Vector2(defaultPos.x, defaultPos.z),
        new Vector2(root.forward.x, root.forward.z)
    );
    }
    public Vector3 RestPosition(Vector3 moveDir, float stride)
    {
        //?
        if (moveDir.magnitude < 0.01f) moveDir = root.forward;

        // 월드 기준 목표물이 있는 방향
        float phi = Vector2.SignedAngle(new Vector2(moveDir.x, moveDir.z), Vector2.up);

        //월드 기준 지금 발이 향할 방향 
        float psi = (theta + phi) * Mathf.Deg2Rad;

        Vector3 raycastOrigin = root.position // 기본 몸통 
         + new Vector3(Mathf.Sin(psi), 100, Mathf.Cos(psi)) // 월드 기준 내 발이 향할 방향
         * 1.2f // 몸통 중심에서 발까지 거리 
         + moveDir.normalized * stride; // 발을 앞으로 좀 뻗어

        Debug.DrawRay(raycastOrigin, Vector3.down * 500, Color.red, 1.0f);

        bool found = Physics.Raycast(
            raycastOrigin, Vector3.down, out RaycastHit rest, 500, groundLayer);
        if (found) return rest.point;
        else
        {
            Debug.Log("couldnt find position");
            return this.transform.position;
        }
    }

    // Vector3 raycastOrigin = targetPos + Vector3.up * 10f;
}

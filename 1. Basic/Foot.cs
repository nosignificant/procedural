using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Foot : MonoBehaviour
{
    public Transform root;
    public Vector3 stablePosition;
    public Vector3 defaultPos;

    public float stride = 10f;

    //0이 왼쪽, 1이 오른쪽

    public int LR;
    public LayerMask groundLayer;

    private float theta;

    void Start()
    {
        stablePosition = this.transform.position;

        //몸통 대비 발의 상대 위치 
        defaultPos = this.transform.position - root.position;

        theta = Vector2.SignedAngle(
        new Vector2(defaultPos.x, defaultPos.z),
        new Vector2(root.forward.x, root.forward.z)
    );
    }
    public Vector3 RestPosition(Vector3 moveDir)
    {
        // 월드 기준 목표물이 있는 방향
        float phi = Vector2.SignedAngle(new Vector2(moveDir.x, moveDir.z), Vector2.up);
        //월드 기준 지금 발이 향할 방향 
        float psi = (theta + phi) * Mathf.Deg2Rad;

        float mag = new Vector2(defaultPos.x, defaultPos.z).magnitude;

        Debug.Log(mag);
        Vector3 raycastOrigin = root.transform.position // 기본 몸통 
         + new Vector3(Mathf.Sin(psi), 100, Mathf.Cos(psi))
         * (mag / 2);// 월드 기준 내 발이 향할 방향


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

}

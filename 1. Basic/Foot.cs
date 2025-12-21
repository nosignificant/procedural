using UnityEngine;
public class Foot : MonoBehaviour
{
    public Transform root;
    public Vector3 stablePosition;
    public float stride = 10f;

    //0이 왼쪽, 1이 오른쪽

    public int LR;
    public LayerMask groundLayer;

    private float theta;
    void Start()
    {
        stablePosition = this.transform.position;
        theta = Vector2.SignedAngle(new Vector2(this.transform.position.x, this.transform.position.z),
                new Vector2(root.forward.x, root.forward.z));
    }
    public Vector3 RestPosition(Vector3 moveDir)
    {
        float phi = Vector2.SignedAngle(new Vector2(moveDir.x, moveDir.z), Vector2.up);
        float psi = (theta + phi) * Mathf.Deg2Rad;
        Vector3 raycastOrigin = root.transform.position // 기본 몸통 
         + new Vector3(Mathf.Sin(psi), 100, Mathf.Cos(psi)) * 3f;

        bool found = Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit rest, 500, groundLayer);
        if (found) return rest.point;
        return this.transform.position;
    }

}

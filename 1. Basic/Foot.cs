using UnityEngine;
public class Foot : MonoBehaviour
{
    public Transform root;
    public Vector3 stablePosition;
    public float stride = 10f;

    //0이 왼쪽, 1이 오른쪽

    public int LR;
    public LayerMask groundLayer;
    void Start()
    {
        stablePosition = this.transform.position;
    }
    public Vector3 RestPosition()
    {
        Vector3 raycastOrigin = this.transform.position // 기본 몸통 
        // + ((LR == 0 ? -root.right : root.right) * 1.5f)// 발이 오른쪽인지 아닌지
         + (root.forward * stride) // 보폭만큼 앞으로 발 뻗음 
         + transform.up * 100;
        bool found = Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit rest, 500, groundLayer);
        if (found) return rest.point;
        return this.transform.position;
    }

}

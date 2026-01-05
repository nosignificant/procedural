using UnityEngine;

public class CCDIKjoint : MonoBehaviour
{
    public Vector3 axis = Vector3.right;
    Vector3 perpendicular; void Start() { perpendicular = axis.Perpendicular(); }
    public bool isTip;

    public void evalute(Transform tip, Transform target)
    {
        //손끝을 목적지로 옮기기 
        transform.rotation = isTip ?
        Quaternion.FromToRotation(tip.up, target.forward)
        : Quaternion.FromToRotation
        (tip.position - transform.position, target.position - transform.position)
         * transform.rotation;

        //관절 제한하기 
        transform.rotation = Quaternion.FromToRotation(transform.rotation * axis,
        transform.parent.rotation * axis
        ) * transform.rotation;
    }
}
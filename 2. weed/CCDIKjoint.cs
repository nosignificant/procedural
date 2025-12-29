public class CCDIKjoint : MonoBehaviour
{
    public Vector3 axis;

    void evalute(transform tip, transform target)
    {
        //손끝을 목적지로 옮기기 
        transform.rotation = Quaternion.FromToRotation
        (tip.position - transform.position,
        target.position - transform.position
        ) * transform.position;

        //관절 제한하기 
        transform.rotation = Quaternion.FromToRotation(

        )

    }
}
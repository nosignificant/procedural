using UnityEngine;

public class CCDIK : MonoBehaviour
{
    [Header("CCDIK")]
    public Transform target;
    public Transform tip;
    public CCDIKjoint[] joints;

    [Header("from to")]

    public Transform from; public Transform to;
    public float criteria = 99999f;
    public bool isFoot = false;
    public LayerMask ground;



    void Update()
    {
        //발이면 레이캐스트 
        if (isFoot)
        {
            target.position = FootUtils.SetTargetGround(tip.position, ground);

            // 거리가 멀면 ik하지 않음 
            //if (FootUtils.isFootFarfromTarget(from.position, to.position, criteria))
            //  return;

        }
        //거리가 가까우면 / isFoot이아니면 ik함 
        foreach (CCDIKjoint joint in joints)
            joint.evalute(tip, target);
    }
}
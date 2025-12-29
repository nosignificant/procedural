using UnityEngine;

public class WeedControl : MonoBehaviour
{
    public transform target;
    public transform tip;

    void Update()
    {
        foreach (CCDIKjoint joint in joints)
        {
            joint.evalute(tip, target, 여기에 뭐들어가지 ?);
        }
    }

}
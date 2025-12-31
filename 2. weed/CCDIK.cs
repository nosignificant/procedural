using UnityEngine;

public class CCDIK : MonoBehaviour
{
    public Transform target;
    public Transform tip;

    public CCDIKjoint[] joints;

    void Update()
    {
        foreach (CCDIKjoint joint in joints)
        {
            joint.evalute(tip, target);
        }
    }

}
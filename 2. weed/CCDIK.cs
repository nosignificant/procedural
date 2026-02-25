using UnityEngine;

public class CCDIK : MonoBehaviour
{
    [Header("CCDIK")]
    public Transform target;
    public Transform tip;
    public CCDIKjoint[] joints;

    public LayerMask ground;

    [Header("orbit setting")]

    public Transform orbitTarget;

    void Update()
    {
        foreach (CCDIKjoint joint in joints)
            joint.evalute(tip, target);
    }

}
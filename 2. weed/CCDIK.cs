using UnityEngine;

public class CCDIK : MonoBehaviour
{
    [Header("CCDIK")]
    public Transform target;
    public Transform tip;
    public CCDIKjoint[] joints;

    public LayerMask ground;

    [Header("orbit setting")]

    public Orbit orbiter;
    public Transform orbitTarget;

    void Start()
    {
        if (orbiter != null) orbiter.SetTarget(orbitTarget);
    }

    void Update()
    {
        foreach (CCDIKjoint joint in joints)
            joint.evalute(tip, target);
    }

    public void SetOrbitTarget(Transform newTarget)
    {
        this.orbitTarget = newTarget;
    }

}
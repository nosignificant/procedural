
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Weedpart2 : MonoBehaviour
{
    float moveDuration = 3f;
    public LayerMask ground;


    public Vector3 SetTargetGround()
    {
        Vector3 raycastOrigin = transform.position + (Vector3.up * 5.0f);
        //Debug.DrawRay(raycastOrigin, Vector3.down * 20f, Color.red, 1.0f);
        bool found = Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit rest, 500, ground);
        if (found) return rest.point;
        else return transform.position;
    }
    public IEnumerator Rotate(Transform root, float radius)
    {

        float time = 0f;

        while (true)
        {
            time += Time.deltaTime / moveDuration;

            float angle = time * 2 * Mathf.PI;

            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;

            Vector3 orbitPos = new Vector3(x, 0, z);

            transform.position = root.position + orbitPos;

            yield return null;
        }
    }
}
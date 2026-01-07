
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Weedpart2 : MonoBehaviour
{
    float moveDuration = 3f;
    public LayerMask ground;
    private float radius = 1f;

    public GameObject root;

    public Vector3 SetTargetGround()
    {

        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        float r = Random.Range(2f, 5f);

        float x = Mathf.Sin(randomAngle) * r;
        float z = Mathf.Cos(randomAngle) * r;

        Vector3 targetPos = root.transform.position + new Vector3(x, 0, z);

        Vector3 rayOrigin = targetPos + (Vector3.up * 10.0f);

        Debug.DrawRay(rayOrigin, Vector3.down * 20f, Color.red, 1.0f);
        bool found = Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit rest, 500, ground);

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
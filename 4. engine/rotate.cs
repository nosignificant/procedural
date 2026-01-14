using UnityEngine;

public class rotate : MonoBehaviour
{
    public float rotateSpeed;
    public Vector3 rotateAxis;
    public float startOffset = 0f;

    private float timer = 0f;
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > startOffset)
            rot();
    }

    void rot()
    {
        transform.Rotate(rotateAxis * rotateSpeed * Time.deltaTime);
    }

    void rotateEuler()
    {
        Vector3 currentRot = transform.eulerAngles;
        currentRot.z += rotateSpeed * Time.deltaTime;
        transform.eulerAngles = currentRot;
    }

}

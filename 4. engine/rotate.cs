using UnityEngine;

public class rotate : MonoBehaviour
{
    public float rotateSpeed;
    public Vector3 rotateAxis;

    // Update is called once per frame
    void Update()
    {
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

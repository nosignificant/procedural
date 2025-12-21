using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerControl : MonoBehaviour
{
    public float speed = 7.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;

    public Transform cameraTransform;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    Vector2 rotation = Vector2.zero;
    public bool getKeyDown = false;

    Foot[] foots;



    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        isKeyDowned();
    }

    public Vector3 GetMove()
    {
        if (characterController.isGrounded)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            float curSpeedZ = speed * Input.GetAxis("Vertical");
            float curSpeedX = speed * Input.GetAxis("Horizontal");

            moveDirection = (forward * curSpeedZ) + (right * curSpeedX);
        }
        return moveDirection;
    }

    public void isKeyDowned()
    {
        getKeyDown = new Vector2(Input.GetAxis("Horizontal"),
        Input.GetAxis("Vertical")).magnitude > 0.1f;
    }
    void Move(Vector3 moveDirection)
    {
        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
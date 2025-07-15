using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2f;
    public float gravity = -9.81f;
    public float crouchHeight = 1f;
    public float normalHeight = 2f;
    public float crouchTransitionSpeed = 6f;

    public AudioSource walkAudioSource;
    public AudioSource sprintAudioSource;

    public AudioClip grassWalk;
    public AudioClip grassSprint;
    public AudioClip concreteWalk;
    public AudioClip concreteSprint;

    Vector3 velocity;
    float targetHeight;
    float heightVelocity;

    void Start()
    {
        targetHeight = controller.height;
    }

    void Update()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && !isCrouching;
        bool isMoving = (x != 0f || z != 0f) && controller.isGrounded;

        float speed = walkSpeed;
        if (isSprinting)
            speed = sprintSpeed;
        else if (isCrouching)
            speed = crouchSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        targetHeight = isCrouching ? crouchHeight : normalHeight;
        controller.height = Mathf.SmoothDamp(controller.height, targetHeight, ref heightVelocity, 1f / crouchTransitionSpeed);

        HandleFootsteps(isMoving, isSprinting);
    }

    void HandleFootsteps(bool isMoving, bool isSprinting)
    {
        if (!isMoving)
        {
            walkAudioSource.Stop();
            sprintAudioSource.Stop();
            return;
        }

        string surface = GetSurfaceTag();
        AudioClip selectedClip = null;

        if (surface == "Grass")
        {
            selectedClip = isSprinting ? grassSprint : grassWalk;
        }
        else if (surface == "Concrete")
        {
            selectedClip = isSprinting ? concreteSprint : concreteWalk;
        }
        else
        {
            selectedClip = isSprinting ? grassSprint : grassWalk;
        }

        AudioSource currentSource = isSprinting ? sprintAudioSource : walkAudioSource;
        AudioSource otherSource = isSprinting ? walkAudioSource : sprintAudioSource;

        if (currentSource.clip != selectedClip)
        {
            currentSource.clip = selectedClip;
            currentSource.Play();
        }

        if (!currentSource.isPlaying)
        {
            currentSource.Play();
        }

        if (otherSource.isPlaying)
        {
            otherSource.Stop();
        }
    }

    string GetSurfaceTag()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            return hit.collider.tag;
        }
        return "Default";
    }
}

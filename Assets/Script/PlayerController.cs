using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float sprintSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 150f;

    private CharacterController cc;
    private float yVelocity;
    private bool isDead;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (anim != null)
            anim.applyRootMotion = false;

        cc = GetComponent<CharacterController>();
        if (cc == null)
        {
            Debug.LogError("CharacterController bulunamadı. Lütfen Player objesine ekleyin.");
            enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (isDead || cc == null || !cc.enabled) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(0f, mouseX, 0f);

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;

        if (move.magnitude > 1f)
            move.Normalize();

        // Shift basılıyken hız artar
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentSpeed = (isSprinting && v > 0f) ? sprintSpeed : speed;

        cc.Move(move * currentSpeed * Time.deltaTime);

        if (cc.isGrounded && yVelocity < 0f)
            yVelocity = -2f;

        if (Input.GetButtonDown("Jump") && cc.isGrounded)
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        yVelocity += gravity * Time.deltaTime;
        cc.Move(Vector3.up * yVelocity * Time.deltaTime);

        if (anim != null)
        {
            float animSpeed = v;
            if (isSprinting && v > 0f)
                animSpeed = 2f;

            anim.SetFloat("Speed", animSpeed);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (anim != null)
        {
            anim.applyRootMotion = false;
            anim.SetFloat("Speed", 0f);
            anim.SetTrigger("Die");
        }

        if (cc != null)
            cc.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        enabled = false;
    }
}
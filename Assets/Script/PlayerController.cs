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

    private Animator anim;

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

        // Mouse ile karakteri sağa sola döndürür
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(0f, mouseX, 0f);

        // Klavye hareketi
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(h, 0f, v);

        if (input.magnitude > 1f)
            input.Normalize();

        Vector3 move = transform.right * input.x + transform.forward * input.z;

        // Sadece ileri giderken Shift ile koşsun
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentSpeed = (isSprinting && v > 0f) ? sprintSpeed : speed;

        cc.Move(move * currentSpeed * Time.deltaTime);

        // Zıplama ve gravity
        if (cc.isGrounded && yVelocity < 0f)
            yVelocity = -2f;

        if (Input.GetButtonDown("Jump") && cc.isGrounded)
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        yVelocity += gravity * Time.deltaTime;
        cc.Move(Vector3.up * yVelocity * Time.deltaTime);

        // Animasyon
        if (anim != null)
        {
            float moveAmount = input.magnitude;

            if (isSprinting && v > 0f)
                moveAmount = 2f;

            anim.SetFloat("Speed", moveAmount);
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
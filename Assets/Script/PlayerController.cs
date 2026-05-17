using UnityEngine;


public class PlayerController : MonoBehaviour
{


    [Header("Movement")]
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 150f;
    private CharacterController cc;
    private float yVelocity;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

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
        if (cc == null) return;

        // Mouse ile sağ-sol dönüş
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(0f, mouseX, 0f);

        // WASD hareketi (karakterin baktığı yöne göre)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = (transform.right * h + transform.forward * v).normalized;
        cc.Move(move * speed * Time.deltaTime);

        // Yerçekimi ve zıplama
        if (cc.isGrounded && yVelocity < 0f)
            yVelocity = -2f;
        if (Input.GetButtonDown("Jump") && cc.isGrounded)
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        yVelocity += gravity * Time.deltaTime;
        cc.Move(Vector3.up * yVelocity * Time.deltaTime);

        float speedValue = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude;
        anim.SetFloat("Speed", speedValue);
    }


}
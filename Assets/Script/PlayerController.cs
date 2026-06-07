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

    // --- SES İÇİN EKLENEN DEĞİŞKENLER ---
    private AudioSource audioSource;
    [Header("Audio Settings")]
    [Tooltip("Yürüme/Koşma ses perdesi hızı (Yürürken standart hızda çalar)")]
    public float walkPitch = 1f;
    [Tooltip("Koşarken sesin ne kadar hızlı/ritmik çalacağını belirler")]
    public float sprintPitch = 1.3f;
    // ------------------------------------

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

        // AudioSource bileşenini alıyoruz
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("Player üzerinde AudioSource bulunamadı! Seslerin çalması için lütfen bir AudioSource ekleyin.");
        }

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

        cameracontroller camCtrl = GetComponentInChildren<cameracontroller>();
        if (camCtrl != null)
            mouseX = camCtrl.ClampHorizontalRotation(mouseX);

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

        // --- SES KONTROLÜ BURADA YAPILIYOR ---
        if (audioSource != null)
        {
            // Şart: Karakter klavyeden girdi alıyor mu (hareket ediyor mu) VE yerde mi?
            if (input.magnitude > 0.1f && cc.isGrounded)
            {
                // Eğer koşuyorsa ses perdesini hızlandır, yürüyorsa normale çek
                if (isSprinting && v > 0f)
                {
                    audioSource.pitch = sprintPitch;
                }
                else
                {
                    audioSource.pitch = walkPitch;
                }

                // Ses zaten çalmıyorsa başlat
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                // Karakter duruyorsa veya havadaysa (zıpladıysa) sesi kes
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // Karakter ölürse sesi anında kes
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

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
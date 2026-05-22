using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [Header("Hedef")]
    public Transform player;

    [Header("Hareket")]
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 5f;
    public float stopDistance = 2f;

    [Header("Yakalama")]
    public float catchDistance = 1.5f;
    public GameManager gameManager;

    [Header("Opsiyonel")]
    public bool lookAtPlayer = true;

    CharacterController cc;
    bool hasCaught;

    void Start()
    {
        cc = GetComponent<CharacterController>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
            else
                Debug.LogWarning("FollowPlayer: 'Player' tag'li obje bulunamadı.");
        }
    }

    void Update()
    {
        if (player == null || hasCaught) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;

        // Yakaladı → kız ölür
        if (distance <= catchDistance)
        {
            CatchPlayer();
            return;
        }

        if (distance <= stopDistance)
            return;

        Vector3 direction = toPlayer.normalized;

        if (lookAtPlayer && direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        Vector3 move = direction * moveSpeed * Time.deltaTime;
        if (cc != null)
            cc.Move(move);
        else
            transform.position += move;
    }
    void CatchPlayer()
    {
        if (hasCaught) return;
        hasCaught = true;

        // Player dur + ölüm animasyonu
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
            pc.enabled = false;

        Animator playerAnim = player.GetComponent<Animator>();
        if (playerAnim != null)
        {
            playerAnim.SetFloat("Speed", 0f);
            playerAnim.SetTrigger("Die");   // ← Sleeping Idle tetiklenir
        }

        // Zombi dur
        Animator zombieAnim = GetComponent<Animator>();
        if (zombieAnim != null)
            zombieAnim.SetFloat("Speed", 0f);
            zombieAnim.SetBool("Bite", true);
        enabled = false;

        // Game Over (panel biraz gecikmeli de olabilir)
        if (gameManager != null)
            gameManager.GameOver();
    }

}
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
    public float biteOffset = 0.8f; // Yakaladığında player'ın ne kadar önüne dursun
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

        // Player ve Enemy collider'ları birbirini görmesin
        // Böylece enemy yaklaşırken player'ı itip öne fırlatmaz
        if (player != null && cc != null)
        {
            CharacterController playerCC = player.GetComponent<CharacterController>();
            if (playerCC != null)
                Physics.IgnoreCollision(cc, playerCC, true);

            Collider playerCol = player.GetComponent<Collider>();
            if (playerCol != null)
                Physics.IgnoreCollision(cc, playerCol, true);
        }
    }

    void Update()
    {
        if (player == null || hasCaught) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;

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

        // Enemy'yi player'ın tam önüne snap'le
        // Hangi yönden geldiyse o yönde dursun, yüzü player'a baksın
        Vector3 fromPlayer = transform.position - player.position;
        fromPlayer.y = 0f;
        if (fromPlayer.sqrMagnitude < 0.0001f)
            fromPlayer = -player.forward;
        Vector3 dir = fromPlayer.normalized;

        Vector3 snapPos = player.position + dir * biteOffset;
        snapPos.y = transform.position.y;
        transform.position = snapPos;

        Vector3 lookTarget = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookTarget);

        // Player'ı durdur + ölüm animasyonu
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
            pc.enabled = false;

        Animator playerAnim = player.GetComponent<Animator>();
        if (playerAnim != null)
        {
            playerAnim.SetFloat("Speed", 0f);
            playerAnim.SetTrigger("Die");
        }

        CharacterController playerCC = player.GetComponent<CharacterController>();
        if (playerCC != null)
            playerCC.enabled = false;

        // Enemy'yi tamamen kilitle: hareket de rotasyon da olmasın
        enabled = false;

        if (cc != null)
            cc.enabled = false;

        Animator zombieAnim = GetComponent<Animator>();
        if (zombieAnim != null)
        {
            zombieAnim.applyRootMotion = false;
            zombieAnim.SetFloat("Speed", 0f);
            zombieAnim.SetBool("Bite", true);
            zombieAnim.CrossFade("Bite", 0.1f);
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        if (gameManager != null)
            gameManager.GameOver();
    }
}
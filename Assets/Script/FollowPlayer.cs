using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [Header("Hedef")]
    public Transform player;

    [Header("Hareket")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public float stopDistance = 1.8f;
    public float gravity = -20f;

    [Header("Algılama")]
    [Tooltip("Açıksa sadece detectionRange içinde kovalar. Kapalıysa her zaman kovalar.")]
    public bool useDetectionLimit = false;
    public float detectionRange = 50f;

    [Tooltip("Sahne açılınca kovalamaya başlamadan önce bekleme süresi")]
    public float chaseStartDelay = 2f;

    [Header("Yakalama")]
    public float catchDistance = 1.3f;
    public float biteOffset = 0.8f;
    public level2Manager gameManager;

    [Header("Animasyon")]
    public string speedParam = "Speed";
    public string biteParam = "Bite";

    [Tooltip("Animator'daki ısırma animasyon state adı. Örn: Bite, AttackL, AttackR 0")]
    public string biteStateName = "AttackL";

    [Header("Opsiyonel")]
    public bool lookAtPlayer = true;

    private CharacterController cc;
    private Animator zombieAnim;
    private Rigidbody rb;

    private bool hasCaught;
    private float chaseAllowedTime;
    private float yVelocity;

    void Start()
    {
        chaseAllowedTime = Time.time + chaseStartDelay;

        cc = GetComponent<CharacterController>();

        zombieAnim = GetComponent<Animator>();
        if (zombieAnim == null)
            zombieAnim = GetComponentInChildren<Animator>();

        if (zombieAnim != null)
            zombieAnim.applyRootMotion = false;

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p != null)
                player = p.transform;
            else
                Debug.LogWarning("FollowPlayer: Player tag'li obje bulunamadı.");
        }

        if (gameManager == null)
            gameManager = FindFirstObjectByType<level2Manager>();

        IgnorePlayerCollision();
    }

    void Update()
    {
        if (player == null || hasCaught) return;

        if (Time.time < chaseAllowedTime)
        {
            SetZombieSpeed(0f);
            ApplyGravityOnly();
            return;
        }

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        float distance = toPlayer.magnitude;

        if (useDetectionLimit && distance > detectionRange)
        {
            SetZombieSpeed(0f);
            ApplyGravityOnly();
            return;
        }

        if (distance <= catchDistance)
        {
            CatchPlayer();
            return;
        }

        if (distance <= stopDistance)
        {
            SetZombieSpeed(0f);
            ApplyGravityOnly();
            return;
        }

        Vector3 direction = toPlayer.normalized;

        if (lookAtPlayer && direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        MoveZombie(direction);
        SetZombieSpeed(moveSpeed);
    }

    void MoveZombie(Vector3 direction)
    {
        if (cc != null && cc.enabled)
        {
            if (cc.isGrounded && yVelocity < 0f)
                yVelocity = -2f;

            yVelocity += gravity * Time.deltaTime;

            Vector3 move = direction * moveSpeed;
            move.y = yVelocity;

            cc.Move(move * Time.deltaTime);
        }
        else
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    void ApplyGravityOnly()
    {
        if (cc != null && cc.enabled)
        {
            if (cc.isGrounded && yVelocity < 0f)
                yVelocity = -2f;

            yVelocity += gravity * Time.deltaTime;

            Vector3 move = new Vector3(0f, yVelocity, 0f);
            cc.Move(move * Time.deltaTime);
        }
    }

    void SetZombieSpeed(float value)
    {
        if (zombieAnim != null)
            zombieAnim.SetFloat(speedParam, value);
    }

    void CatchPlayer()
    {
        if (hasCaught || player == null) return;

        hasCaught = true;

        SetZombieSpeed(0f);

        Vector3 fromPlayer = transform.position - player.position;
        fromPlayer.y = 0f;

        if (fromPlayer.sqrMagnitude < 0.0001f)
            fromPlayer = -player.forward;

        Vector3 dir = fromPlayer.normalized;

        Vector3 snapPos = player.position + dir * biteOffset;
        snapPos.y = transform.position.y;
        transform.position = snapPos;

        Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookPos);

        StopZombieMovement();

        PlayBiteAnimation();

        KillPlayer();

        Camera playerCam = player.GetComponentInChildren<Camera>();
        if (playerCam != null)
            playerCam.transform.SetParent(null);

        if (gameManager != null)
            gameManager.GameOver();
        else
            Debug.LogWarning("FollowPlayer: level2Manager atanmadı!");

        enabled = false;
    }

    void PlayBiteAnimation()
    {
        if (zombieAnim == null) return;

        zombieAnim.applyRootMotion = false;
        zombieAnim.SetFloat(speedParam, 0f);

        bool foundBiteParam = false;

        foreach (AnimatorControllerParameter param in zombieAnim.parameters)
        {
            if (param.name == biteParam)
            {
                foundBiteParam = true;

                if (param.type == AnimatorControllerParameterType.Bool)
                    zombieAnim.SetBool(biteParam, true);
                else if (param.type == AnimatorControllerParameterType.Trigger)
                    zombieAnim.SetTrigger(biteParam);

                break;
            }
        }

        if (!foundBiteParam)
            Debug.LogWarning("Animator'da '" + biteParam + "' isimli parametre yok.");

        if (!string.IsNullOrEmpty(biteStateName))
        {
            zombieAnim.Play(biteStateName, 0, 0f);
        }
    }

    void KillPlayer()
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc == null)
            pc = player.GetComponentInChildren<PlayerController>();

        if (pc != null)
        {
            pc.Die();
            return;
        }

        Animator playerAnim = player.GetComponentInChildren<Animator>();
        if (playerAnim != null)
        {
            playerAnim.applyRootMotion = false;
            playerAnim.SetFloat("Speed", 0f);

            foreach (AnimatorControllerParameter param in playerAnim.parameters)
            {
                if (param.name == "isDead" && param.type == AnimatorControllerParameterType.Bool)
                    playerAnim.SetBool("isDead", true);

                if (param.name == "Die" && param.type == AnimatorControllerParameterType.Trigger)
                    playerAnim.SetTrigger("Die");
            }
        }

        CharacterController playerCC = player.GetComponent<CharacterController>();
        if (playerCC == null)
            playerCC = player.GetComponentInChildren<CharacterController>();

        if (playerCC != null)
            playerCC.enabled = false;

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb == null)
            playerRb = player.GetComponentInChildren<Rigidbody>();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.useGravity = false;
            playerRb.isKinematic = true;
        }
    }

    void StopZombieMovement()
    {
        if (cc != null)
            cc.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }

            agent.enabled = false;
        }
    }

    void IgnorePlayerCollision()
    {
        if (player == null) return;

        Collider[] zombieCols = GetComponentsInChildren<Collider>();
        Collider[] playerCols = player.GetComponentsInChildren<Collider>();

        foreach (Collider zCol in zombieCols)
        {
            if (zCol == null || zCol.isTrigger) continue;

            foreach (Collider pCol in playerCols)
            {
                if (pCol == null || pCol.isTrigger) continue;

                Physics.IgnoreCollision(zCol, pCol, true);
            }
        }
    }
}
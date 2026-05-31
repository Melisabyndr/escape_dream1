using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WolfAttack : MonoBehaviour
{
    [Header("Hedef")]
    public Transform player;

    [Header("Hareket")]
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 5f;

    [Header("Yakalama")]
    public float catchDistance = 1.5f;
    public float biteOffset = 0.8f;
    public GameManager gameManager;
    public float gameOverDelay = 1.5f;
    public float wolfFreezeDelay = 1f;

    [Header("Animasyon")]
    public string speedParam = "Speed";
    public string biteParam = "Bite";
    public string biteStateName = "AttackR 0";

    CharacterController cc;
    Animator wolfAnim;
    bool hasCaught;

    void Start()
    {
        cc = GetComponent<CharacterController>();

        wolfAnim = GetComponent<Animator>();
        if (wolfAnim == null)
            wolfAnim = GetComponentInChildren<Animator>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
            else
                Debug.LogWarning("WolfAttack: Player tag'li obje bulunamadı.");
        }

        IgnorePlayerCollisions();

        if (wolfAnim != null)
        {
            wolfAnim.applyRootMotion = false;
            wolfAnim.SetBool(biteParam, false);
            wolfAnim.SetFloat(speedParam, 0f);
        }
    }

    void IgnorePlayerCollisions()
    {
        if (player == null) return;

        Collider[] wolfCols = GetComponentsInChildren<Collider>();
        Collider[] playerCols = player.GetComponentsInChildren<Collider>();

        foreach (Collider wc in wolfCols)
        {
            foreach (Collider pc in playerCols)
            {
                if (wc != null && pc != null)
                    Physics.IgnoreCollision(wc, pc, true);
            }
        }

        CharacterController playerCC = player.GetComponent<CharacterController>();
        if (playerCC == null)
            playerCC = player.GetComponentInChildren<CharacterController>();

        if (cc != null && playerCC != null)
            Physics.IgnoreCollision(cc, playerCC, true);
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

        Vector3 direction = toPlayer.normalized;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        Vector3 move = direction * moveSpeed * Time.deltaTime;

        if (cc != null && cc.enabled)
            cc.Move(move);
        else
            transform.position += move;

        SetSpeed(moveSpeed);
    }

    void CatchPlayer()
    {
        if (hasCaught || player == null) return;
        hasCaught = true;

        // 1. Player'ı durdur
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc == null)
            pc = player.GetComponentInChildren<PlayerController>();
        if (pc != null)
            pc.Die();
        else
        {
            CharacterController playerCC = player.GetComponent<CharacterController>();
            if (playerCC == null)
                playerCC = player.GetComponentInChildren<CharacterController>();
            if (playerCC != null)
                playerCC.enabled = false;
        }

        // 2. Kurt hareketini durdur
        if (cc != null)
            cc.enabled = false;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null && agent.isActiveAndEnabled)
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }
            agent.enabled = false;
        }

        Rigidbody wolfRb = GetComponent<Rigidbody>();
        if (wolfRb != null)
        {
            wolfRb.linearVelocity = Vector3.zero;
            wolfRb.angularVelocity = Vector3.zero;
            wolfRb.isKinematic = true;
        }

        // 3. Konumlandır
        Vector3 fromPlayer = transform.position - player.position;
        fromPlayer.y = 0f;
        if (fromPlayer.sqrMagnitude < 0.0001f)
            fromPlayer = -player.forward;

        Vector3 dir = fromPlayer.normalized;
        Vector3 snapPos = player.position + dir * biteOffset;
        snapPos.y = transform.position.y;
        transform.position = snapPos;

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        Vector3 lookDir = transform.position - player.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
            player.rotation = Quaternion.LookRotation(lookDir);

        // 4. Player animasyonu + kamera sabitle
        Animator playerAnim = player.GetComponentInChildren<Animator>();
        if (playerAnim != null)
        {
            playerAnim.applyRootMotion = false;
            playerAnim.SetFloat("Speed", 0f);
            playerAnim.SetTrigger("Die");
        }

        Camera playerCam = player.GetComponentInChildren<Camera>();
        if (playerCam != null)
            playerCam.transform.SetParent(null);

        // 5. Kurt animasyonu
        SetSpeed(0f);

        if (wolfAnim != null)
        {
            wolfAnim.applyRootMotion = false;
            wolfAnim.speed = 1f;
            wolfAnim.SetFloat(speedParam, 0f);
            wolfAnim.SetBool(biteParam, true);
            wolfAnim.CrossFade(biteStateName, 0.1f);
        }

        // 6. Coroutine'ler (enabled = false kullanma, coroutine'leri öldürür)
        StartCoroutine(FreezeWolfAfterBite());
        StartCoroutine(DelayedGameOver());
    }

    IEnumerator FreezeWolfAfterBite()
    {
        yield return new WaitForSeconds(wolfFreezeDelay);

        if (wolfAnim != null)
        {
            wolfAnim.speed = 0f;
            wolfAnim.SetFloat(speedParam, 0f);
            wolfAnim.SetBool(biteParam, true);
        }
    }

    IEnumerator DelayedGameOver()
    {
        yield return new WaitForSeconds(gameOverDelay);

        if (gameManager != null)
            gameManager.GameOver();
        else
            Debug.LogWarning("WolfAttack: GameManager Inspector'da atanmadı!");
    }

    void SetSpeed(float value)
    {
        if (wolfAnim != null)
            wolfAnim.SetFloat(speedParam, value);
    }
}
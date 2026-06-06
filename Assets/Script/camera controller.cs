using UnityEngine;

public class cameracontroller : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = new Vector3(0f, 3f, -5f);
    public float lookHeight = 2.2f;
    public float smoothSpeed = 12f;

    [Header("Duvar çarpışması")]
    public float collisionRadius = 0.3f;
    public float wallOffset = 0.25f;
    public float minDistance = 1.2f;
    public LayerMask collisionLayers = ~0;

    [Header("Harita sınırı (opsiyonel)")]
    public bool useBounds = false;
    public Vector2 minXZ = new Vector2(-400f, -400f);
    public Vector2 maxXZ = new Vector2(400f, 400f);

    bool frozen;
    float currentDistance;

    void Start()
    {
        currentDistance = offset.magnitude;
    }

    public void FreezeCamera()
    {
        frozen = true;
    }

    void LateUpdate()
    {
        if (frozen || target == null) return;

        Vector3 lookPoint = target.position + Vector3.up * lookHeight;
        Vector3 desiredOffset = target.rotation * offset.normalized;
        float maxDistance = offset.magnitude;

        Vector3 desiredPosition = lookPoint + desiredOffset * maxDistance;

        // Duvar kontrolü — player'dan kameraya doğru
        Vector3 dir = (desiredPosition - lookPoint).normalized;
        float targetDistance = maxDistance;

        if (Physics.SphereCast(
            lookPoint,
            collisionRadius,
            dir,
            out RaycastHit hit,
            maxDistance,
            collisionLayers,
            QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.transform.IsChildOf(target))
                targetDistance = Mathf.Max(minDistance, hit.distance - wallOffset);
        }

        // Yumuşak mesafe (duvara yapışınca zıplamasın)
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, smoothSpeed * Time.deltaTime);

        desiredPosition = lookPoint + dir * currentDistance;

        // Harita kenarı sınırı
        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minXZ.x, maxXZ.x);
            desiredPosition.z = Mathf.Clamp(desiredPosition.z, minXZ.y, maxXZ.y);
        }

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
        transform.LookAt(lookPoint);
    }
}
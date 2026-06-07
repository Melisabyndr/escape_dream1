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
    public float minDistance = 1.5f;
    public LayerMask collisionLayers = ~0;

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

    /// <summary>
    /// PlayerController mouse X buradan geçmeli.
    /// Duvar arkasına kamera gidecekse dönüşü kısar.
    /// </summary>
    public float ClampHorizontalRotation(float mouseXDegrees)
    {
        if (target == null || Mathf.Approximately(mouseXDegrees, 0f))
            return mouseXDegrees;

        float currentY = target.eulerAngles.y;

        // Tam dönüş serbest mi?
        if (!IsCameraBlockedAtYaw(currentY + mouseXDegrees))
            return mouseXDegrees;

        // Adım adım azalt (duvara değene kadar izin ver)
        float sign = Mathf.Sign(mouseXDegrees);
        float abs = Mathf.Abs(mouseXDegrees);
        float allowed = 0f;

        for (float step = abs; step >= 0.01f; step -= 0.5f)
        {
            if (!IsCameraBlockedAtYaw(currentY + sign * step))
            {
                allowed = sign * step;
                break;
            }
        }

        return allowed;
    }

    bool IsCameraBlockedAtYaw(float yaw)
    {
        Vector3 lookPoint = target.position + Vector3.up * lookHeight;
        Vector3 dir = (Quaternion.Euler(0f, yaw, 0f) * offset).normalized;
        float maxDist = offset.magnitude;

        if (!Physics.SphereCast(
            lookPoint,
            collisionRadius,
            dir,
            out RaycastHit hit,
            maxDist,
            collisionLayers,
            QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        if (hit.collider.transform.IsChildOf(target))
            return false;

        float freeDistance = hit.distance - wallOffset;
        return freeDistance < minDistance;
    }

    void LateUpdate()
    {
        if (frozen || target == null) return;

        Vector3 lookPoint = target.position + Vector3.up * lookHeight;
        Vector3 dir = (Quaternion.Euler(0f, target.eulerAngles.y, 0f) * offset).normalized;
        float maxDistance = offset.magnitude;
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

        currentDistance = Mathf.Lerp(
            currentDistance,
            targetDistance,
            smoothSpeed * Time.deltaTime
        );

        Vector3 desiredPosition = lookPoint + dir * currentDistance;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
        transform.LookAt(lookPoint);
    }
}
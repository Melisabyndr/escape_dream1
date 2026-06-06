using UnityEngine;

public class cameracontroller : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = new Vector3(0f, 3f, -5f);
    public float lookHeight = 2.2f;
    public float smoothSpeed = 10f;

    public float collisionRadius = 0.4f;
    public float wallOffset = 0.3f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 lookPoint = target.position + Vector3.up * lookHeight;
        Vector3 desiredPosition = target.position + target.rotation * offset;

        Vector3 dir = desiredPosition - lookPoint;
        float dist = dir.magnitude;
        dir.Normalize();

        RaycastHit hit;

        if (Physics.SphereCast(lookPoint, collisionRadius, dir, out hit, dist))
        {
            if (!hit.collider.transform.IsChildOf(target))
            {
                desiredPosition = hit.point - dir * wallOffset;
            }
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(lookPoint);
    }
}
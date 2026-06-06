using UnityEngine;

public class GasCollect : MonoBehaviour
{
    public AudioClip gasPickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gasPickupSound != null)
            {
                // transform.position yerine Camera.main.transform.position yazdık
                // En sona da ses seviyesini maksimum (1.0f) yapan bir çarpan ekledik
                AudioSource.PlayClipAtPoint(gasPickupSound, Camera.main.transform.position, 1.0f);
            }

            Game.Instance.CollectGas();
            Destroy(gameObject);
        }
    }
}
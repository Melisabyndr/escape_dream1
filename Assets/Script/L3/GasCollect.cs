using UnityEngine;

public class GasCollect : MonoBehaviour
{
    // Unity Inspector panelinden ses dosyasını buraya sürükleyip bırakacaksın
    public AudioClip gasPickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Eğer bir ses dosyası atandıysa, benzinin olduğu pozisyonda sesi çalar
            if (gasPickupSound != null)
            {
                AudioSource.PlayClipAtPoint(gasPickupSound, transform.position);
            }

            // Senin mevcut benzin toplama kodun
            Game.Instance.CollectGas();

            // Benzin bidonunu sahneden yok et
            Destroy(gameObject);
        }
    }
}
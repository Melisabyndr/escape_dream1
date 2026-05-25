using UnityEngine;
using System.Collections;

public class ZombieKill : MonoBehaviour
{
    public level2Manager gameManager;

    private bool oldu = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !oldu)
        {
            oldu = true;

            // Karakteri düşür
            Animator anim = other.GetComponent<Animator>();

            if (anim != null)
            {
                anim.SetTrigger("Die");
            }

            // Biraz bekle sonra yazıyı göster
            StartCoroutine(OlumGecikmesi());
        }
    }

    IEnumerator OlumGecikmesi()
    {
        yield return new WaitForSeconds(2f);

        gameManager.GameOver();
    }
}
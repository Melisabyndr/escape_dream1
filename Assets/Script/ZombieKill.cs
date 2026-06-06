using UnityEngine;
using System.Collections;
using UnityEditor.Search;

public class ZombieKill : MonoBehaviour
{
    public level2Manager gameManager;
    public AudioSource Hırıltılı;
    public AudioSource yeme;

    private bool oldu = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !oldu)
        {
            oldu = true;

            if (Hırıltılı != null && Hırıltılı.isPlaying)
            {
                Hırıltılı.Stop();
            }

            if (yeme != null)
            {
                yeme.Play();
            }

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
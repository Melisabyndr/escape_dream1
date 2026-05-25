using UnityEngine;
using System.Collections;

public class ExitDoor : MonoBehaviour
{
    public level2Manager gameManager;

    private bool kazandi = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !kazandi)
        {
            kazandi = true;

            // Kapı animasyonu
            Animator anim = GetComponent<Animator>();

            if (anim != null)
            {
                anim.SetTrigger("Open");
            }

            StartCoroutine(KazanmaGecikmesi());
        }
    }

    IEnumerator KazanmaGecikmesi()
    {
        // Kapı açılmasını bekle
        yield return new WaitForSeconds(2f);

        gameManager.WinGame();
    }
}
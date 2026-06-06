using UnityEngine;
using System.Collections; // Coroutine kullanabilmek için bu kütüphane şart

public class Door : MonoBehaviour
{
    public Transform door;     // iç kapı
    public GameObject qText;   // Press Q yazısı
    public GameObject messageText; // Anahtar gerekli yazısı

    public bool hasKey = false;
    private bool playerNear;
    public bool isOpen = false;

    void Start()
    {
        if (qText != null) qText.SetActive(false);
        if (messageText != null) messageText.SetActive(false);
    }

    void Update()
    {
        // Kapı zaten açıksa tekrar Q'ya basılmasın
        if (playerNear && Input.GetKeyDown(KeyCode.Q) && !isOpen)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        // Q'ya basıldığı an "Q'ya bas" yazısını hemen gizliyoruz
        if (qText != null)
            qText.SetActive(false);

        // --- ANAHTAR YOKSA ---
        if (!hasKey)
        {
            if (messageText != null)
                messageText.SetActive(true);

            // Eski Coroutine'leri durdurup yenisini başlatıyoruz (Üst üste basılırsa bug olmasın diye)
            StopAllCoroutines();
            StartCoroutine(HideMessageRoutine());

            return; // Fonksiyonu burada kes, kapıyı açma
        }

        // --- ANAHTAR VARSA (Kapıyı Açma Mantığı) ---
        if (door != null)
            door.localRotation = Quaternion.Euler(0, 90, 0);

        isOpen = true;

        if (FindFirstObjectByType<GameManager>() != null)
            FindFirstObjectByType<GameManager>().WinGame();
    }

    // Yazıları sırayla kapatıp açan Coroutine yöntemi
    IEnumerator HideMessageRoutine()
    {
        // 2 saniye bekle (Anahtar gerekli yazısı ekranda kalır)
        yield return new WaitForSeconds(2f);

        // Anahtar gerekli yazısını kapat
        if (messageText != null)
            messageText.SetActive(false);

        // Eğer oyuncu hala kapının yanındaysa, "Q'ya bas" yazısını geri getir
        if (playerNear && qText != null)
            qText.SetActive(true);
    }

    public void GiveKey()
    {
        hasKey = true;
        Debug.Log("Anahtar alındı!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            playerNear = true;
            if (qText != null)
                qText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;

            // Alandan çıkınca tüm yazıları sıfırla
            if (qText != null) qText.SetActive(false);
            if (messageText != null) messageText.SetActive(false);

            StopAllCoroutines(); // Alandan çıkınca zamanlayıcıyı durdur
        }
    }
}
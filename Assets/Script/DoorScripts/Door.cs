using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public Transform door;     // iç kapı
    public GameObject qText;   // Kapıyı Aç yazısı
    public GameObject messageText; // Anhatar gerekli

    public AudioSource doorSound; // Kapı sesi

    public bool hasKey = false; // Oyuncunun anahtarı var mı kontrol

    private bool playerNear; // oyuncu kapıya yakın mı
    public bool isOpen = false; // kapı hiç açıldı mı

    void Start()
    {// yazıyı gizler
        if (qText != null)
            qText.SetActive(false); 

        if (messageText != null)
            messageText.SetActive(false);
    }

    void Update()
    {// oyuncu yakınsa yazı çıkar
        if (playerNear && Input.GetKeyDown(KeyCode.Q))
        {
            TryOpenDoor();
        }
    }

    void TryOpenDoor()
    {
        if (isOpen) return;//kapı açıksa tekrar çalışmaz

        if (qText != null)
            qText.SetActive(false);

        if (!hasKey)
        {
            ShowMessage();
            return;
        }

        OpenDoor();
    }
    void OpenDoor()
    {
        if (door != null)
            door.localRotation = Quaternion.Euler(0, 90, 0);

        // Kapı açılınca ses çal
        if (doorSound != null)
            doorSound.Play();

        isOpen = true;

        Debug.Log("Kapı açıldı!");

        if (messageText != null)
            messageText.SetActive(false);

        FindFirstObjectByType<GameManager>().WinGame();
    }

    void ShowMessage() // anahtar gerekli
    {
        if (messageText != null)
            messageText.SetActive(true);

        StartCoroutine(HideMessage());
    }

    IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(2f);

        if (messageText != null)
            messageText.SetActive(false);
    }

    public void GiveKey()
    {
        hasKey = true;
        Debug.Log("Anahtar alındı!");
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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

            if (qText != null)
                qText.SetActive(false);
        }
    }
}

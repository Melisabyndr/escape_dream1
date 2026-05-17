using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform door;     // iç kapı
    public GameObject qText;   // Press Q yazısı
    public GameObject messageText;

    public bool hasKey = false;

    private bool playerNear;
    public bool isOpen = false;

    void Start()
    {
        if (qText != null)
            qText.SetActive(false);

        if (messageText != null)
            messageText.SetActive(false);
    }

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.Q))
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        if (!hasKey)
        {
            if (messageText != null)
                messageText.SetActive(true);
            StartCoroutine(HideMessage());

            System.Collections.IEnumerator HideMessage()
            {
                yield return new WaitForSeconds(2f);

                if (messageText != null)
                    messageText.SetActive(false);
            }

            return;
        }

        if (!hasKey)
        {
            Debug.Log("Kapıyı açmak için anahtar gerekli!");
            return;
        }

        if (isOpen) return;

        if (door != null)
            door.localRotation = Quaternion.Euler(0, 90, 0);

        isOpen = true;
        FindFirstObjectByType<GameManager>().WinGame();

        if (qText != null)
            qText.SetActive(false);

    }

    public void GiveKey()
    {
        hasKey = true;
        Debug.Log("Anahtar alındı!");
    }


    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger çalıştı");
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

    //Invoke("HideMessage", 2f);

    void HideMessage()
    {
        if (messageText != null)
            messageText.SetActive(false);
    }
}

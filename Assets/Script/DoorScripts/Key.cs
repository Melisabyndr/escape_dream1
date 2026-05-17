using UnityEngine;

public class Key : MonoBehaviour
{
    public GameObject qText;
    public Door door;

    private bool isPlayerNear;

    void Start()
    {
        if (qText != null)
            qText.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Q))
        {
            PickKey();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (qText != null) qText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (qText != null) qText.SetActive(false);
        }
    }

    void PickKey()
    {
        if (door != null)
        {
            door.GiveKey(); // kapıya anahtar veriyoruz
        }

        if (qText != null)
            qText.SetActive(false);

        Destroy(gameObject);
    }
}

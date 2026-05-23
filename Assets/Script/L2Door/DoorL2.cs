using UnityEngine;
using System.Collections;

public class DoorL2 : MonoBehaviour
{
    public Transform doorModel; // dönen kapı
    public GameObject qText;
    public GameObject jumpscareImage;
    public AudioSource zombieSound; // zombi sesi
    public AudioSource doorSound; // kapı sesi

    public bool isCorrectDoor = false;

    private bool playerNear = false;
    private bool isOpen = false;

    IEnumerator ShowJumpscare()
    {

        if (jumpscareImage != null)
            jumpscareImage.SetActive(true);

        if (zombieSound != null)
            zombieSound.Play();

        yield return new WaitForSeconds(1.5f);

        if (jumpscareImage != null)
            jumpscareImage.SetActive(false);

    }

    void Start()
    {
        if (qText != null)
            qText.SetActive(false);
    }

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.Q))
        {
            if (qText != null)
                qText.SetActive(false);

            TryOpenDoor();
        }
    }

    void TryOpenDoor()
    {
        if (isOpen) return;

        if (isCorrectDoor)
        {
            OpenDoor();
        }
        else
        {
            StartCoroutine(ShowJumpscare());
        }
    }

    void OpenDoor()
    {
        doorModel.localRotation = Quaternion.Euler(0, 90, 0);
        isOpen = true;

        if (qText != null)
            qText.SetActive(false);

        if (doorSound != null)
            doorSound.Play();
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



using System.Collections;
using UnityEngine;

public class BusDoor : MonoBehaviour
{
    [Header("UI")]
    public GameObject interactText;
    public GameObject notEnoughGasText;

    [Header("Doors (PIVOTS)")]
    // BURAYA UNITY'DEN O OLUŞTURDUĞUMUZ PIVOT OBJELERİNİ ATAYACAKSINIZ
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool playerNear = false;
    private bool doorOpened = false;

    private Quaternion leftClosedRot;
    private Quaternion rightClosedRot;
    private Quaternion leftOpenRot;
    private Quaternion rightOpenRot;

    private void Start()
    {
        // Kapıların başlangıç rotasyonunu kaydet
        leftClosedRot = leftDoor.localRotation;
        rightClosedRot = rightDoor.localRotation;

        // İÇE AÇILMASI İÇİN İŞARETLERİ DEĞİŞTİRDİK (-openAngle ve +openAngle yaptık)
        leftOpenRot = leftClosedRot * Quaternion.Euler(0, -openAngle, 0);
        rightOpenRot = rightClosedRot * Quaternion.Euler(0, openAngle, 0);
    }

    private void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.Q))
        {
            interactText.SetActive(false);

            if (Game.Instance.collectedGas >= 20)
            {
                OpenDoor();
            }
            else
            {
                // Eğer zaten çalışıyorsa coroutine'i üst üste başlatmamak için kontrol
                StopAllCoroutines();
                StartCoroutine(ShowNoGasMessage());
            }
        }

        if (doorOpened)
        {
            // Yumuşak geçiş ile hedef lokal rotasyona dönme
            leftDoor.localRotation = Quaternion.Slerp(
                leftDoor.localRotation,
                leftOpenRot,
                Time.deltaTime * openSpeed
            );

            rightDoor.localRotation = Quaternion.Slerp(
                rightDoor.localRotation,
                rightOpenRot,
                Time.deltaTime * openSpeed
            );
        }
    }

    void OpenDoor()
    {
        if (doorOpened) return;

        doorOpened = true;
        Debug.Log("Kapılar içe doğru açıldı!");
        StartCoroutine(WinAfterDoor());
        IEnumerator WinAfterDoor()
        {
            yield return new WaitForSeconds(1f); // kapı açılma hissi

            FindFirstObjectByType<Level3Manager>().WinGame();
        }
    }

    IEnumerator ShowNoGasMessage()
    {
        notEnoughGasText.SetActive(true);
        yield return new WaitForSeconds(2f);
        notEnoughGasText.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            interactText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            interactText.SetActive(false);
        }
    }
}
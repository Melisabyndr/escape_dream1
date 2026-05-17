using UnityEngine;

public class FinishZone : MonoBehaviour
{
    private GameManager gameManager;
    private Door door; // kapı scriptini tutacak

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        door = FindFirstObjectByType<Door>(); // Door scriptinin adı neyse onu yaz
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && door.isOpen)
        {
            gameManager.WinGame();
        }
    }
}

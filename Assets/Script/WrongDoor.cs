using UnityEngine;

public class WrongDoor : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.GameOver();
        }
    }
}
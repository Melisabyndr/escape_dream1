using UnityEngine;

public class DogHit : MonoBehaviour
{
    public Level3Manager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Çarpılan obje: " + other.name);
        Debug.Log("Tag: " + other.tag);

        if (other.CompareTag("Player"))
        {
            Debug.Log("OYUNCUYA DEĞDİ!");
            gameManager.GameOver();
        }
    }
}
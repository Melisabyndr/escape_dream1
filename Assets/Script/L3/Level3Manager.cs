using UnityEngine;

public class Level3Manager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject winPanel;

    bool oyunBitti = false;

    public void GameOver()
    {
        if (oyunBitti) return;

        oyunBitti = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void WinGame()
    {
        Debug.Log("WIN GAME ÇALIŞTI - KİM ÇAĞIRDI?");
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
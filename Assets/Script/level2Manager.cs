using UnityEngine;
using UnityEngine.SceneManagement;

public class level2Manager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject winPanel;

    void Start()
    {
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void WinGame()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
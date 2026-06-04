using UnityEngine;
using UnityEngine.SceneManagement;

public class level2Manager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject winPanel;

    [Header("Restart")]
    [Tooltip("Build Settings'teki sahne2 index'i (genelde 2)")]
    public int restartSceneBuildIndex = 2;

    public bool isGameOver = false;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (winPanel != null)
            winPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void WinGame()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame (sahne2) - sahne yükleniyor, index: " + restartSceneBuildIndex);

        isGameOver = false;
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (winPanel != null)
            winPanel.SetActive(false);

        SceneManager.LoadScene(restartSceneBuildIndex, LoadSceneMode.Single);
    }

    /// <summary>
    /// Eski buton bağlantıları RetryGame kullanıyorsa çalışmaya devam eder.
    /// </summary>
    public void RetryGame()
    {
        RestartGame();
    }
}
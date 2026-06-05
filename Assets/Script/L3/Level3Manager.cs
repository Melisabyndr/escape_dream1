using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3Manager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject winPanel;

    [Header("Sahne başı koruma")]
    [Tooltip("sahne2'den gelince köpekler bu süre boyunca öldürmez")]
    public float spawnGraceDuration = 3f;

    public static float GraceEndsAt;

    [Header("Restart")]
    [Tooltip("Açıksa: aktif sahne. Kapalıysa aşağıdaki index (sahne3 = 4)")]
    public bool reloadCurrentSceneOnRestart = true;

    [Tooltip("reloadCurrentSceneOnRestart kapalıysa. Build Settings sırasına göre")]
    public int restartSceneBuildIndex = 4;

    public bool isGameOver = false;

    void Start()
    {
        GraceEndsAt = Time.unscaledTime + spawnGraceDuration;

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
        Debug.Log("Level3Manager.RestartGame");

        isGameOver = false;
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (winPanel != null)
            winPanel.SetActive(false);

        if (reloadCurrentSceneOnRestart)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        else
            SceneManager.LoadScene(restartSceneBuildIndex, LoadSceneMode.Single);
    }

    public void MainMenu()
    {
        isGameOver = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}